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

namespace MusicLyrics
{
    public class MusicManager
    {
        const string LyricsSite = "https://www.utamap.com/";
        const string LyricsSearch_URL = "https://www.utamap.com/searchkasi.php?searchname={0}&word={1}";
        const string LyricsSearch_Title = "title";
        const string LyricsSearch_Artist = "artist";
        const string FirstLyricsURLNode = "/html/body/table[3]/tr/td/table[1]/tr/td/a";
        const string LyricsNode = "/html/body/div/div/table[2]/tr/td/table[3]/tr/td/table/tr[4]/td";

        public static async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties()
        {
            GlobalSystemMediaTransportControlsSessionManager sessions = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            return await sessions.GetCurrentSession().TryGetMediaPropertiesAsync();
        }

        public static async Task<string> GetMusicLyrics(GlobalSystemMediaTransportControlsSessionMediaProperties properties)
        {
            string url = string.Format(LyricsSearch_URL, LyricsSearch_Title, properties.Title);
            HtmlDocument document = await GetHtml(url);

            var lyricsURLNode = document.DocumentNode.SelectSingleNode(FirstLyricsURLNode);
            if (lyricsURLNode != null)
            {
                string lyricsURL = lyricsURLNode.GetAttributeValue("href", string.Empty);
                if (lyricsURL.Length > 0)
                {
                    lyricsURL = LyricsSite + lyricsURL;
                    HtmlDocument lyricsDocument = await GetHtml(lyricsURL);

                    var lyricsNode = lyricsDocument.DocumentNode.SelectSingleNode(LyricsNode);
                    if (lyricsNode != null)
                    {
                        Regex pattern = new Regex("<br>");
                        string lyrics = pattern.Replace(lyricsNode.InnerHtml, "\n");
                        return lyrics.Remove(lyrics.LastIndexOf("<!-- 歌詞 end -->")).Trim();
                    }
                }
            }
            return null;
        }

        static async Task<HtmlDocument> GetHtml(string url)
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
