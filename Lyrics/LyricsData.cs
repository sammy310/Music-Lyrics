using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLyrics.Lyrics
{
    public class LyricsData
    {
        public string Title { get; set; }

        public string Artist { get; set; }

        public string Lyrics { get; set; }

        public LyricsData() { }

        public LyricsData(string title, string artist)
        {
            Title = title;
            Artist = artist;
        }

        public LyricsData(string title, string artist, string lyrics)
        {
            Title = title;
            Artist = artist;
            Lyrics = lyrics;
        }

        public LyricsData(LyricsData lyricsData)
        {
            if (lyricsData != null)
            {
                Title = lyricsData.Title;
                Artist = lyricsData.Artist;
                Lyrics = lyricsData.Lyrics;
            }
        }
    }
}
