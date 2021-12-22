﻿using System;
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

namespace MusicLyrics
{
    public sealed partial class LyricsPage : Page
    {
        public List<string> LyricsSites { get; } = new List<string>();

        private int currentSelectSiteIndex => comboBoxSearchSite.SelectedIndex;
        public SiteData CurrentSelectSite => MusicManager.Instance.GetSiteData(currentSelectSiteIndex);

        public List<SearchTypes> SearchOptions { get; } = new List<SearchTypes>(Enum.GetValues(typeof(SearchTypes)).Cast<SearchTypes>());


        private static bool IsFirstStart { get; set; } = true;

        private static string TItleText { get; set; } = null;
        private static string ArtistText { get; set; } = null;
        private static BitmapImage Thumbnail { get; set; } = null;
        private static string LyricsText { get; set; } = null;

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
                if (TItleText != null)
                {
                    titleText.Text = TItleText;
                }
                if (ArtistText != null)
                {
                    artistText.Text = ArtistText;
                }
                if (Thumbnail != null)
                {
                    thumbnail.Source = Thumbnail;
                }
                if (LyricsText != null)
                {
                    lyricsText.Text = LyricsText;
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

            TItleText = titleText.Text = mediaProperties.Title;
            ArtistText = artistText.Text = mediaProperties.Artist;

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
            GetMusicLyrics(searchType, searchValue, false);
        }

        private async void GetMusicLyrics(SearchTypes searchType, string searchValue, bool justGetFirst)
        {
            lyricsText.Text = "Searching...";

            SearchTypes[] searchTypes = { searchType };
            string lyrics = await MusicManager.Instance.GetMusicLyrics(searchValue, CurrentSelectSite, searchTypes, justGetFirst);
            SetLyricsText(lyrics);
        }

        private void SetLyricsText(string newLyrics)
        {
            if (newLyrics != null)
            {
                LyricsText = lyricsText.Text = newLyrics;
            }
            else
            {
                lyricsText.Text = "No results found";
            }
        }

        private async void GetAllDefaultInfo()
        {
            if (await GetMusicInfo() == false) return;
            GetMusicLyrics(SearchTypes.Title, titleText.Text, true);
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
    }
}