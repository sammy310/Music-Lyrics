using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLyrics
{
    public class MusicData
    {
        public string Title => GetProperty("title");

        public string Artist => GetProperty("artist");

        public string URL => GetProperty("url");

        public string Lyrics => GetProperty("lyrics");

        public List<Tuple<string, string>> Properties { get; } = new List<Tuple<string, string>>();

        public SiteData TargetSiteData { get; private set; } = null;


        public void AddProperty(string name, string value)
        {
            Properties.Add(new Tuple<string, string>(name, value));
        }

        public void SetTargetSiteData(SiteData siteData)
        {
            TargetSiteData = siteData;
        }

        public string GetProperty(string name)
        {
            return Properties.Find((it) => it.Item1.CompareTo(name) == 0)?.Item2;
        }
    }
}
