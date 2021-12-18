using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;

namespace MusicLyrics
{
    public class SiteData
    {
        public enum SelectTypes
        {
            Attribute, InnerHtml,
        }

        public bool IsRootSite => Name != null;

        public string Name { get; private set; }

        public string URL { get; private set; }
        public int URLFormattingSize { get; private set; }

        public int SearchOptionSize => URLFormattingSize - 1;
        private List<List<Tuple<SearchOption, string>>> searchOptions = null;

        public string XPath { get; private set; }

        public SelectTypes SelectType { get; private set; }
        public string SelectValue { get; private set; }

        public bool HasReplace => ReplaceRegex != null;
        public string ReplaceRegex { get; private set; }
        public string ReplaceValue { get; private set; }

        public bool HasRemove => RemoveValue != null;
        public string RemoveValue { get; private set; }

        public bool HasNextSite => NextSite != null;
        public SiteData NextSite { get; private set; } = null;


        public SiteData(XmlNode node)
        {
            Name = node.Attributes["name"]?.Value ?? null;

            XmlNode urlNode = node.SelectSingleNode("URL");
            URL = urlNode.InnerText;
            URLFormattingSize = int.Parse(urlNode.Attributes["size"].Value);

            if (SearchOptionSize > 0)
            {
                searchOptions = new List<List<Tuple<SearchOption, string>>>();

                XmlNode searchNode = node.SelectSingleNode("SearchOption");
                for (int i = 1; i < URLFormattingSize; i++)
                {
                    searchOptions.Add(new List<Tuple<SearchOption, string>>());

                    XmlNode sNode = searchNode.SelectSingleNode("Option" + i);
                    foreach (string option in sNode.InnerText.Split('|'))
                    {
                        string[] opt = option.Split(':');
                        SearchOption searchOption = SearchOption.Title;
                        switch(opt[0])
                        {
                            case "Title":
                                searchOption = SearchOption.Title;
                                break;
                            case "Artist":
                                searchOption = SearchOption.Artist;
                                break;
                        }
                        searchOptions[searchOptions.Count - 1].Add(new Tuple<SearchOption, string>(searchOption, opt[1]));
                    }
                }
            }

            XmlNode selectNode = node.SelectSingleNode("Select");

            XPath = selectNode.SelectSingleNode("XPath").InnerText;

            XmlNode typeNode = selectNode.SelectSingleNode("Type");
            switch (typeNode.Attributes["type"].Value)
            {
                case "Attribute":
                    SelectType = SelectTypes.Attribute;
                    SelectValue = typeNode.InnerText;
                    break;
                case "InnerHtml":
                    SelectType = SelectTypes.InnerHtml;
                    break;
            }

            XmlNode replaceNode = selectNode.SelectSingleNode("Replace");
            if (replaceNode != null)
            {
                ReplaceRegex = Regex.Unescape(replaceNode.InnerText);
                ReplaceValue = Regex.Unescape(replaceNode.Attributes["value"].Value);
            }

            XmlNode removeNode = selectNode.SelectSingleNode("Remove");
            if (removeNode != null)
            {
                RemoveValue = Regex.Unescape(removeNode.InnerText);
            }

            XmlNode nextNode = selectNode.SelectSingleNode("Site");
            if (nextNode != null)
            {
                NextSite = new SiteData(nextNode);
            }
        }


        public List<Tuple<SearchOption, string>> GetSearchOptions(int index)
        {
            return searchOptions[index];
        }

        public string GetSearchOption(int index, SearchOption searchOption)
        {
            foreach (Tuple<SearchOption, string> option in searchOptions[index])
            {
                if (option.Item1 == searchOption) return option.Item2;
            }
            return null;
        }
    }
}
