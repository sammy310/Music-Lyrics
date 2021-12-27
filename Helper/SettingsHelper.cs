using System;
using System.Collections.Generic;
using System.IO;
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

        private const string LyricsSitesDataPathKey = "LyricsSitesDataPath";

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

            object lyricsSitesDataPath = ApplicationData.Current.LocalSettings.Values[LyricsSitesDataPathKey];
            if (lyricsSitesDataPath != null)
            {
                lyricsSitesDataFilePath = (string)lyricsSitesDataPath;
            }
            else
            {
                InitLyricsSitesDataFile();
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


        // Lyrics Sites Data

        public const string DefaultLyricsSitesDataFileName = "LyricsSite.xml";
        private static string lyricsSitesDataFilePath = DefaultLyricsSitesDataFileName;
        public static string LyricsSitesDataFilePath
        {
            get => lyricsSitesDataFilePath;
            set
            {
                if (value != null)
                {
                    if (MusicManager.Instance.InitLyricsSites(value))
                    {
                        lyricsSitesDataFilePath = value;
                        ApplicationData.Current.LocalSettings.Values[LyricsSitesDataPathKey] = lyricsSitesDataFilePath;
                    }
                }
            }
        }

        public static void InitLyricsSitesDataFile()
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MusicLyrics.Lyrics.LyricsSite.xml"))
            {
                string path = ApplicationData.Current.LocalFolder.Path + "\\" + DefaultLyricsSitesDataFileName;
                using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(file);
                }
                LyricsSitesDataFilePath = path;
            }
        }
    }
}
