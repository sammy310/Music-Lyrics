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

        public int SearchTypeSize => URLFormattingSize - 1;
        private List<List<Tuple<SearchTypes, string>>> searchTypes = null;

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

            if (SearchTypeSize > 0)
            {
                searchTypes = new List<List<Tuple<SearchTypes, string>>>();

                XmlNode searchNode = node.SelectSingleNode("SearchOption");
                for (int i = 1; i < URLFormattingSize; i++)
                {
                    searchTypes.Add(new List<Tuple<SearchTypes, string>>());

                    XmlNode sNode = searchNode.SelectSingleNode("Option" + i);
                    foreach (string option in sNode.InnerText.Split('|'))
                    {
                        string[] opt = option.Split(':');
                        SearchTypes searchType = SearchTypes.Title;
                        switch(opt[0])
                        {
                            case "Title":
                                searchType = SearchTypes.Title;
                                break;
                            case "Artist":
                                searchType = SearchTypes.Artist;
                                break;
                        }
                        searchTypes[searchTypes.Count - 1].Add(new Tuple<SearchTypes, string>(searchType, opt[1]));
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


        public List<Tuple<SearchTypes, string>> GetSearchTypes(int index)
        {
            return searchTypes[index];
        }

        public string GetSearchValue(int index, SearchTypes searchType)
        {
            foreach (Tuple<SearchTypes, string> option in searchTypes[index])
            {
                if (option.Item1 == searchType) return option.Item2;
            }
            return null;
        }
    }
}
