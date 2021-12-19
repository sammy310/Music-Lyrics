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
using Windows.UI.Xaml.Navigation;


// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace MusicLyrics
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<string> LyricsSites { get; private set; } = new List<string>();

        private int currentSelectSiteIndex => LyricsSite.SelectedIndex;
        public SiteData CurrentSelectSite => MusicManager.Instance.GetSiteData(currentSelectSiteIndex);


        public MainPage()
        {
            this.InitializeComponent();

            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(680, 720));

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            SetLyricsSites();
        }

        private void SetLyricsSites()
        {
            foreach (SiteData data in MusicManager.Instance.SiteDatas)
            {
                LyricsSites.Add(data.Name);
            }
        }

        private async Task<bool> GetMusicInfo()
        {
            var mediaProperties = await MusicManager.GetMediaProperties();
            if (mediaProperties == null) return false;

            if (mediaProperties.Thumbnail != null)
            {
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(await mediaProperties.Thumbnail.OpenReadAsync());
                thumbnail.Source = image;
            }

            titleText.Text = mediaProperties.Title;
            artistText.Text = mediaProperties.Artist;

            titleTextBox.Text = mediaProperties.Title;
            artistTextBox.Text = mediaProperties.Artist;

            return true;
        }

        private void GetMusicLyricsWithSearchOption()
        {
            if (titleTextBox.Text.Length > 0)
            {
                GetMusicLyrics(SearchOption.Title, titleTextBox.Text);
            }
            else if (artistTextBox.Text.Length > 0)
            {
                GetMusicLyrics(SearchOption.Artist, artistTextBox.Text);
            }
        }

        private async void GetMusicLyrics(SearchOption searchOption, string searchValue)
        {
            SearchOption[] searchOptions = { searchOption };
            string lyrics = await MusicManager.GetMusicLyrics(searchValue, CurrentSelectSite, searchOptions);
            lyricsText.Text = lyrics ?? "No results found";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (await GetMusicInfo() == false) return;

            SearchOption[] searchOptions = { SearchOption.Title };
            string lyrics = await MusicManager.GetMusicLyrics(titleText.Text, CurrentSelectSite, searchOptions);
            if (lyrics != null)
            {
                lyricsText.Text = lyrics;
            }
        }

        private async void GetMusicInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (await GetMusicInfo() == false) return;
        }

        private void GetMusicLyricsButton_Click(object sender, RoutedEventArgs e)
        {
            GetMusicLyricsWithSearchOption();
        }

        private void LyricsSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
