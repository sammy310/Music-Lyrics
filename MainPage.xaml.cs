using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public MainPage()
        {
            this.InitializeComponent();

            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(650, 720));

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var mediaProperties = await MusicManager.GetMediaProperties();

            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(await mediaProperties.Thumbnail.OpenReadAsync());
            thumbnail.Source = image;

            titleText.Text = mediaProperties.Title;
            artistText.Text = mediaProperties.Artist;

            string lyrics = await MusicManager.GetMusicLyrics(mediaProperties);
            if (lyrics != null)
            {
                lyricsText.Text = lyrics;
            }
        }
    }
}
