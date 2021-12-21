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
        private const string IsGetLyricsOnStartKey = "IsGetlyricsOnStart";

        private const string LyricsFontSizeKey = "LyricsFontSize";
        public const double DefaultFontSize = 20;

        public static void Initialize()
        {
            object isGetLyricsOnStartValue = ApplicationData.Current.LocalSettings.Values[IsGetLyricsOnStartKey];
            if (isGetLyricsOnStartValue != null)
            {
                isGetLyricsOnStart = (bool)isGetLyricsOnStartValue;
            }

            object lyricsFontSizeValue = ApplicationData.Current.LocalSettings.Values[LyricsFontSizeKey];
            if (lyricsFontSizeValue != null)
            {
                lyricsFontSize = (double)lyricsFontSizeValue;
            }
        }

        private static bool isGetLyricsOnStart = true;
        public static bool IsGetLyricsOnStart
        {
            get => isGetLyricsOnStart;
            set
            {
                if (isGetLyricsOnStart != value)
                {
                    isGetLyricsOnStart = value;
                    ApplicationData.Current.LocalSettings.Values[IsGetLyricsOnStartKey] = isGetLyricsOnStart;
                }
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
