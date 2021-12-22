using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicLyrics.Lyrics
{
    public sealed partial class LyricsPage : Page
    {
        public List<string> LyricsSites { get; } = new List<string>();

        private int currentSelectSiteIndex => comboBoxSearchSite.SelectedIndex;
        public SiteData CurrentSelectSite => MusicManager.Instance.GetSiteData(currentSelectSiteIndex);

        public List<SearchTypes> SearchOptions { get; } = new List<SearchTypes>(Enum.GetValues(typeof(SearchTypes)).Cast<SearchTypes>());


        private static bool IsFirstStart { get; set; } = true;

        private static LyricsData CurrentLyricsData { get; set; } = new LyricsData();

        private static BitmapImage Thumbnail { get; set; } = null;

        private static string SearchTitleText { get; set; } = null;
        private static string SearchArtistText { get; set; } = null;


        public LyricsPage()
        {
            this.InitializeComponent();

            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(680, 720));

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            SetLyricsSites();

            Initialize();
        }

        private void SetLyricsSites()
        {
            foreach (SiteData data in MusicManager.Instance.SiteDatas)
            {
                LyricsSites.Add(data.Name);
            }
        }

        private void Initialize()
        {
            if (!IsFirstStart)
            {
                if (CurrentLyricsData != null)
                {
                    titleText.Text = CurrentLyricsData.Title;
                    artistText.Text = CurrentLyricsData.Artist;
                    lyricsText.Text = CurrentLyricsData.Lyrics;
                }

                if (Thumbnail != null)
                {
                    thumbnail.Source = Thumbnail;
                }

                if (SearchTitleText != null)
                {
                    searchTitleTextBox.Text = SearchTitleText;
                }
                if (SearchArtistText != null)
                {
                    searchArtistTextBox.Text = SearchArtistText;
                }
            }
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsFirstStart)
            {
                IsFirstStart = false;
                if (SettingsHelper.IsGetLyricsOnStart)
                {
                    GetAllDefaultInfo();
                }
            }
        }

        private void LyricsText_Loaded(object sender, RoutedEventArgs e)
        {
            lyricsText.FontSize = SettingsHelper.LyricsFontSize;
        }


        private async Task<bool> GetMusicInfo()
        {
            var mediaProperties = await MusicManager.Instance.GetMediaProperties();
            if (mediaProperties == null) return false;

            if (mediaProperties.Thumbnail != null)
            {
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(await mediaProperties.Thumbnail.OpenReadAsync());
                thumbnail.Source = image;
                Thumbnail = image;
            }

            SetTitleText(mediaProperties.Title);
            SetArtistText(mediaProperties.Artist);

            SearchTitleText = searchTitleTextBox.Text = mediaProperties.Title;
            SearchArtistText = searchArtistTextBox.Text = mediaProperties.Artist;

            return true;
        }

        private void GetMusicLyricsWithSearchOption()
        {
            SearchTypes searchType = (SearchTypes)comboBoxSearchType.SelectedValue;
            string searchValue = string.Empty;
            if (searchType == SearchTypes.Title)
            {
                if (searchTitleTextBox.Text.Length > 0) searchValue = searchTitleTextBox.Text;
            }
            else if (searchType == SearchTypes.Artist)
            {
                if (searchArtistTextBox.Text.Length > 0) searchValue = searchArtistTextBox.Text;
            }
            GetMusicLyrics(searchType, searchValue, true);
        }

        private async void GetMusicLyrics(SearchTypes searchType, string searchValue, bool isSelectLyrics)
        {
            lyricsText.Text = "Searching...";

            SearchTypes[] searchTypes = { searchType };
            LyricsData lyricData = await MusicManager.Instance.GetMusicLyrics(null, searchValue, CurrentSelectSite, searchTypes, !isSelectLyrics);
            SetLyricsData(lyricData, !isSelectLyrics);
        }

        private void SetTitleText(string newText)
        {
            CurrentLyricsData.Title = titleText.Text = newText;
        }

        private void SetArtistText(string newText)
        {
            CurrentLyricsData.Artist = artistText.Text = newText;
        }

        private void SetLyricsData(LyricsData newLyricsData, bool onlyChangeLyrics)
        {
            if (newLyricsData != null)
            {
                if (!onlyChangeLyrics)
                {
                    SetTitleText(newLyricsData.Title);
                    SetArtistText(newLyricsData.Artist);
                }
                CurrentLyricsData.Lyrics = lyricsText.Text = newLyricsData.Lyrics;
            }
            else
            {
                lyricsText.Text = "No results found";
            }
        }

        private async void GetAllDefaultInfo()
        {
            if (await GetMusicInfo() == false) return;
            GetMusicLyrics(SearchTypes.Title, titleText.Text, false);
        }

        private void GetAllInfoButton_Click(object sender, RoutedEventArgs e)
        {
            GetAllDefaultInfo();
        }

        private async void GetMusicInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (await GetMusicInfo() == false) return;
        }

        private void GetMusicLyricsButton_Click(object sender, RoutedEventArgs e)
        {
            GetMusicLyricsWithSearchOption();
        }

        private void SearchPanelBack_Click(object sender, RoutedEventArgs e)
        {
            toggleSearchOption.IsOn = !toggleSearchOption.IsOn;
        }

        private void SplitView_PaneClosed(SplitView sender, object args)
        {
            toggleSearchOption.IsOn = false;
        }

        private void SearchTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTitleText = searchTitleTextBox.Text;
        }

        private void SearchArtistTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchArtistText = searchArtistTextBox.Text;
        }
    }
}
