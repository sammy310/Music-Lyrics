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

namespace MusicLyrics.Navigation
{
    public sealed partial class NavigationPage : Page
    {
        List<NavigationItemData> NavigationList = new List<NavigationItemData>()
        {
            new NavigationItemData("Lyrics", typeof(LyricsPage), Symbol.MusicInfo),
        };

        public NavigationPage()
        {
            this.InitializeComponent();

            NavigationInitialize();
        }

        private void NavigationInitialize()
        {
            foreach (NavigationItemData itemData in NavigationList)
            {
                NavigationViewItem item = new NavigationViewItem();
                item.Content = itemData.Name;
                item.Tag = itemData.PageType;
                item.Icon = new SymbolIcon(itemData.IconSymbol);

                navigationView.MenuItems.Add(item);
            }

            navigationView.SelectedItem = navigationView.MenuItems[0];

            navigationView.Resources.SetValue(HeightProperty, 30);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                rootFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                NavigationViewItem item = (NavigationViewItem)args.SelectedItem;
                rootFrame.Navigate((Type)item.Tag);
            }

            navigationView.AlwaysShowHeader = args.IsSettingsSelected;
            navigationView.Header = ((NavigationViewItem)navigationView.SelectedItem).Content.ToString();
        }
    }
}
