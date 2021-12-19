using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Windows.Media.Control;
using Windows.UI.Xaml.Controls;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web;

namespace MusicLyrics
{
    public enum SearchOption
    {
        Title, Artist,
    }

    public sealed class MusicManager
    {
        static readonly Lazy<MusicManager> lazy = new Lazy<MusicManager>(() => new MusicManager());
        public static MusicManager Instance { get { return lazy.Value; } }

        public List<SiteData> SiteDatas { get; private set; } = null;


        public MusicManager()
        {
            InitLyricsSites();
        }

        private void InitLyricsSites()
        {
            SiteDatas = new List<SiteData>();

            XmlDocument siteXml = new XmlDocument();
            siteXml.Load(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MusicLyrics.LyricsSite.xml"));
            
            XmlNode sitesNode = siteXml.SelectSingleNode("Sites");
            foreach (XmlNode site in sitesNode.SelectNodes("Site"))
            {
                SiteDatas.Add(new SiteData(site));
            }
        }

        public SiteData GetSiteData(int index)
        {
            return SiteDatas[index];
        }

        public static async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties()
        {
            GlobalSystemMediaTransportControlsSessionManager sessions = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            GlobalSystemMediaTransportControlsSession currentSesseion = sessions.GetCurrentSession();
            return currentSesseion == null ? null : await currentSesseion.TryGetMediaPropertiesAsync();
        }

        public static async Task<string> GetMusicLyrics(string searchValue, SiteData siteData, SearchOption[] searchOptions)
        {
            string url;

            // Set URL
            if (siteData.SearchOptionSize > 0)
            {
                List<string> args = new List<string>();
                args.Add(searchValue);
                for (int i = 0; i < searchOptions.Length; i++)
                {
                    args.Add(siteData.GetSearchOption(i, searchOptions[i]));
                }
                url = string.Format(siteData.URL, args.ToArray());
            }
            else
            {
                url = string.Format(siteData.URL, searchValue);
            }
            HtmlDocument document = await GetHtml(url);

            // Select XPath
            HtmlNode selectNode = document.DocumentNode.SelectSingleNode(siteData.XPath);
            if (selectNode != null)
            {
                // Select
                string selectStr = string.Empty;
                if (siteData.SelectType == SiteData.SelectTypes.Attribute)
                {
                    selectStr = selectNode.GetAttributeValue(siteData.SelectValue, string.Empty).Trim();
                }
                else if (siteData.SelectType == SiteData.SelectTypes.InnerHtml)
                {
                    selectStr = selectNode.InnerHtml.Trim();
                }

                if (selectStr.Length > 0)
                {
                    // Replace
                    if (siteData.HasReplace)
                    {
                        Regex pattern = new Regex(siteData.ReplaceRegex);
                        selectStr = pattern.Replace(selectStr, siteData.ReplaceValue).Trim();
                    }

                    // Remove
                    if (siteData.HasRemove)
                    {
                        selectStr = selectStr.Remove(selectStr.IndexOf(siteData.RemoveValue)).Trim();
                    }

                    // Html Decode
                    selectStr = HttpUtility.HtmlDecode(selectStr);

                    // Return or Move Next Site
                    if (siteData.HasNextSite)
                    {
                        return await GetMusicLyrics(selectStr, siteData.NextSite, null);
                    }
                    else
                    {
                        return selectStr;
                    }
                }
            }
            return null;
        }

        private static async Task<HtmlDocument> GetHtml(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                HttpContent content = response.Content;

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(await content.ReadAsStringAsync());

                return document;
            }
        }
    }
}
