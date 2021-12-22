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
using Windows.UI.Xaml.Navigation;

namespace MusicLyrics
{
    public sealed partial class LyricsSelectPage : Page
    {
        public List<MusicData> MusicDatas { get; private set; }

        public int SelectedIndex => listView.SelectedIndex;

        public LyricsSelectPage(List<MusicData> musicDatas)
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.SizeChanged += CoreWindow_SizeChanged;

            SetMaximumPageSize();

            MusicDatas = musicDatas;
        }

        private void CoreWindow_SizeChanged(Windows.UI.Core.CoreWindow sender, object args)
        {
            SetMaximumPageSize();
        }

        private void SetMaximumPageSize()
        {
            this.Width = Math.Min((double)Application.Current.Resources["ContentDialogMaxWidth"], Window.Current.Bounds.Width) * 0.9;
            this.Height = Math.Min((double)Application.Current.Resources["ContentDialogMaxHeight"], Window.Current.Bounds.Height) * 0.9;
        }
    }
}
