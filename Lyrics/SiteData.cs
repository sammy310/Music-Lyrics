using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;

namespace MusicLyrics
{
    public enum XPathTypes
    {
        Search, Select,
    }

    public enum XPathSelectTypes
    {
        Node, Attribute, InnerHtml, InnerText,
    }

    public class XPathProperty
    {
        public string Name { get; private set; }

        public string XPath { get; private set; }

        public XPathSelectTypes XPathSelectType { get; private set; }
        public string XPathSelectValue { get; private set; }

        public List<XPathProperty> Properties { get; private set; }

        public bool HasReplace => ReplaceRegex != null;
        public string ReplaceRegex { get; private set; }
        public string ReplaceValue { get; private set; }

        public bool HasRemove => RemoveValue != null;
        public string RemoveValue { get; private set; }

        public XPathProperty(XmlNode node)
        {
            Name = node.Attributes["name"]?.Value ?? null;

            XPath = node.SelectSingleNode("XPath").InnerText;

            XmlNode typeNode = node.SelectSingleNode("Type");
            if (typeNode == null)
            {
                XPathSelectType = XPathSelectTypes.Node;
            }
            else
            {
                switch (typeNode.InnerText)
                {
                    case "Attribute":
                        XPathSelectType = XPathSelectTypes.Attribute;
                        XPathSelectValue = typeNode.Attributes["value"].Value;
                        break;
                    case "InnerHtml":
                        XPathSelectType = XPathSelectTypes.InnerHtml;
                        break;
                    case "InnerText":
                        XPathSelectType = XPathSelectTypes.InnerText;
                        break;
                }
            }

            XmlNode replaceNode = node.SelectSingleNode("Replace");
            if (replaceNode != null)
            {
                ReplaceRegex = Regex.Unescape(replaceNode.InnerText);
                ReplaceValue = Regex.Unescape(replaceNode.Attributes["value"].Value);
            }

            XmlNode removeNode = node.SelectSingleNode("Remove");
            if (removeNode != null)
            {
                RemoveValue = Regex.Unescape(removeNode.InnerText);
            }

            foreach (XmlNode n in node.SelectNodes("Property"))
            {
                AddProperty(new XPathProperty(n));
            }
        }

        public void AddProperty(XPathProperty property)
        {
            if (Properties == null)
            {
                Properties = new List<XPathProperty>();
            }
            Properties.Add(property);
        }
    }

    public class SiteData
    {
        public bool IsValidData { get; private set; } = true;

        public bool IsRootSite => Name != null;

        public string Name { get; private set; }

        public string URL { get; private set; }
        public int URLFormattingSize { get; private set; }

        public int SearchTypeSize => URLFormattingSize - 1;
        private List<List<Tuple<SearchTypes, string>>> searchTypes = null;

        public XPathTypes XPathType { get; private set; }
        public XPathProperty Property { get; private set; } = null;

        public string XPath => Property.XPath;
        public XPathSelectTypes XPathSelectType => Property.XPathSelectType;
        public string XPathSelectValue => Property.XPathSelectValue;

        public bool HasNextSite => NextSite != null;
        public SiteData NextSite { get; private set; } = null;


        public SiteData(XmlNode node)
        {
            Name = node.Attributes["name"]?.Value ?? null;

            XmlNode urlNode = node.SelectSingleNode("URL");
            if (urlNode != null)
            {
                URL = urlNode.InnerText;
                URLFormattingSize = int.Parse(urlNode.Attributes["size"].Value);
            }
            else
            {
                IsValidData = false;
                return;
            }

            if (SearchTypeSize > 0)
            {
                searchTypes = new List<List<Tuple<SearchTypes, string>>>();

                XmlNode searchOptionNode = node.SelectSingleNode("SearchOption");
                for (int i = 1; i < URLFormattingSize; i++)
                {
                    searchTypes.Add(new List<Tuple<SearchTypes, string>>());

                    XmlNode sNode = searchOptionNode.SelectSingleNode("Option" + i);
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

            // XPath Search Type
            XmlNode searchNode = node.SelectSingleNode("Search");
            if (searchNode != null)
            {
                XPathType = XPathTypes.Search;
                Property = new XPathProperty(searchNode);

                XmlNode nextNode = searchNode.SelectSingleNode("Site");
                if (nextNode != null)
                {
                    NextSite = new SiteData(nextNode);
                }
            }

            // XPath Select Type
            XmlNode selectNode = node.SelectSingleNode("Select");
            if (selectNode != null)
            {
                XPathType = XPathTypes.Select;
                Property = new XPathProperty(selectNode);
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
