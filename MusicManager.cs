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
using MusicLyrics.Lyrics;

namespace MusicLyrics
{
    public enum SearchTypes
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
            siteXml.Load(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MusicLyrics.Lyrics.LyricsSite.xml"));
            
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

        public async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties()
        {
            GlobalSystemMediaTransportControlsSessionManager sessions = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            GlobalSystemMediaTransportControlsSession currentSesseion = sessions.GetCurrentSession();
            return currentSesseion == null ? null : await currentSesseion.TryGetMediaPropertiesAsync();
        }

        public async Task<LyricsData> GetMusicLyrics(LyricsData lyricsData, string searchValue, SiteData siteData, SearchTypes[] searchTypes, bool justGetFirst)
        {
            string url;

            // Set URL
            if (siteData.SearchTypeSize > 0)
            {
                List<string> args = new List<string>();
                args.Add(searchValue);
                for (int i = 0; i < searchTypes.Length; i++)
                {
                    args.Add(siteData.GetSearchValue(i, searchTypes[i]));
                }
                url = string.Format(siteData.URL, args.ToArray());
            }
            else
            {
                url = string.Format(siteData.URL, searchValue);
            }
            HtmlDocument document = await GetHtml(url);
            if (document == null)
            {
                return null;
            }


            if (siteData.XPathType == XPathTypes.Search)
            {
                HtmlNodeCollection selectNodes = document.DocumentNode.SelectNodes(siteData.XPath);
                if (selectNodes != null)
                {
                    List<MusicData> musicDatas = new List<MusicData>();
                    foreach (HtmlNode selectNode in selectNodes)
                    {
                        MusicData musicData = new MusicData();
                        foreach (XPathProperty property in siteData.Property.Properties)
                        {
                            string propertyValue = SelectXPath(selectNode, property);
                            if (propertyValue != null)
                            {
                                musicData.AddProperty(property.Name, propertyValue);
                            }
                        }
                        if (siteData.HasNextSite)
                        {
                            musicData.SetTargetSiteData(siteData.NextSite);
                        }
                        musicDatas.Add(musicData);
                    }

                    MusicData selectMusicData = await SelectLyricsFromUser(musicDatas, justGetFirst);
                    if (selectMusicData != null)
                    {
                        return await GetMusicLyrics(selectMusicData);
                    }
                }
            }
            else if (siteData.XPathType == XPathTypes.Select)
            {
                HtmlNode selectNode = document.DocumentNode.SelectSingleNode(siteData.XPath);
                if (selectNode != null)
                {
                    string lyrics = SelectXPath(selectNode, siteData.Property);
                    LyricsData returnData = new LyricsData(lyricsData);
                    returnData.Lyrics = lyrics;
                    return returnData;
                }
            }
            return null;
        }

        public async Task<LyricsData> GetMusicLyrics(MusicData musicData)
        {
            return await GetMusicLyrics(new LyricsData(musicData.Title, musicData.Artist), musicData.URL, musicData.TargetSiteData, null, true);
        }

        private async Task<HtmlDocument> GetHtml(string url)
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

        private string SelectXPath(HtmlNode node, XPathProperty property)
        {
            HtmlNode selectNode = node.SelectSingleNode(property.XPath);
            if (selectNode == null) return null;

            string selectStr = string.Empty;
            switch (property.XPathSelectType)
            {
                case XPathSelectTypes.Attribute:
                    selectStr = selectNode.GetAttributeValue(property.XPathSelectValue, string.Empty).Trim();
                    break;
                case XPathSelectTypes.InnerHtml:
                    selectStr = selectNode.InnerHtml.Trim();
                    break;
                case XPathSelectTypes.InnerText:
                    selectStr = selectNode.InnerText.Trim();
                    break;
            }

            if (selectStr.Length > 0)
            {
                // Replace
                if (property.HasReplace)
                {
                    Regex pattern = new Regex(property.ReplaceRegex);
                    selectStr = pattern.Replace(selectStr, property.ReplaceValue).Trim();
                }

                // Remove
                if (property.HasRemove)
                {
                    int index = selectStr.IndexOf(property.RemoveValue);
                    if (index != -1)
                    {
                        selectStr = selectStr.Remove(index, property.RemoveValue.Length).Trim();
                    }
                }

                // Html Decode
                selectStr = HttpUtility.HtmlDecode(selectStr);
            }

            return selectStr;
        }

        private async Task<MusicData> SelectLyricsFromUser(List<MusicData> musicDatas, bool justGetFirst)
        {
            int selectIndex = 0;
            if (musicDatas.Count > 1 && justGetFirst == false)
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Select Lyrics";
                dialog.PrimaryButtonText = "Select";
                dialog.CloseButtonText = "Cancel";
                dialog.DefaultButton = ContentDialogButton.Primary;
                LyricsSelectPage selectPage = new LyricsSelectPage(musicDatas);
                dialog.Content = selectPage;

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    selectIndex = Math.Max(0, selectPage.SelectedIndex);
                }
                else
                {
                    return null;
                }
            }
            return musicDatas[selectIndex];
        }
    }
}
