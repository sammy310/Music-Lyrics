using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MusicLyrics
{
    public static class SettingsHelper
    {
        private const string LyricsFontSizeKey = "LyricsFontSize";
        public const double DefaultFontSize = 20;

        public static void Initialize()
        {
            object value = ApplicationData.Current.LocalSettings.Values[LyricsFontSizeKey];
            if (value != null)
            {
                lyricsFontSize = (double)value;
            }
        }

        private static double lyricsFontSize = DefaultFontSize;
        public static double LyricsFontSize
        {
            get => lyricsFontSize;
            set
            {
                if (lyricsFontSize != value)
                {
                    lyricsFontSize = value;
                    ApplicationData.Current.LocalSettings.Values[LyricsFontSizeKey] = lyricsFontSize;
                }
            }
        }
    }
}
