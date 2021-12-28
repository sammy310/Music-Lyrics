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
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MusicLyrics
{
    public sealed partial class SettingsPage : Page
    {

        // Font
        public List<double> FontSizes { get; } = new List<double>()
        {
            8,
            9,
            10,
            11,
            12,
            14,
            16,
            18,
            20,
            24,
            28,
            36,
            48,
            72
        };
        public const double MinFontSize = 8;
        public const double MaxFontSize = 100;


        public SettingsPage()
        {
            this.InitializeComponent();
        }


        private bool CheckFontSize(double newSize)
        {
            return FontSizes.Contains(newSize) || (newSize < MaxFontSize && newSize > MinFontSize);
        }

        private void ComboFontSize_Loaded(object sender, RoutedEventArgs e)
        {
            comboFontSize.SelectedValue = SettingsHelper.LyricsFontSize;
        }

        private void ComboFontSize_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            bool isDouble = double.TryParse(sender.Text, out double newSize);

            if (isDouble && CheckFontSize(newSize))
            {
                sender.SelectedValue = newSize;
            }
            else
            {
                sender.Text = sender.SelectedValue.ToString();
            }

            args.Handled = true;
        }

        private void ComboFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isDouble = double.TryParse(comboFontSize.Text, out double fontSize);
            if (isDouble)
            {
                SettingsHelper.LyricsFontSize = fontSize;
                textFontSizeExample.FontSize = fontSize;
            }
        }

        private void ToggleLyricsOnStart_Loaded(object sender, RoutedEventArgs e)
        {
            toggleLyricsOnStart.IsOn = SettingsHelper.IsGetLyricsOnStart;
        }

        private void ToggleLyricsOnStart_Toggled(object sender, RoutedEventArgs e)
        {
            SettingsHelper.IsGetLyricsOnStart = toggleLyricsOnStart.IsOn;
        }


        // Lyrics Sites

        //private async void ButtonLyricsSitesData_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = new FileOpenPicker();
        //    picker.ViewMode = PickerViewMode.List;
        //    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        //    picker.FileTypeFilter.Add(".xml");

        //    StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        textBoxLyricsSitesData.Text = file.Name;
        //        SettingsHelper.LyricsSitesDataFilePath = file.Path;
        //        if (SettingsHelper.LyricsSitesDataFilePath.CompareTo(file.Path) == 0)
        //        {
        //            //textBoxLyricsSitesData.Text = file.Name;
        //            textBoxLyricsSitesData.Text = file.Path;
        //        }
        //        // Wrong file
        //        else
        //        {
        //            var dialog = new ContentDialog();
        //            dialog.CloseButtonText = "OK";
        //            dialog.DefaultButton = ContentDialogButton.Close;
        //            dialog.Title = "File data incorrect!!";
        //            await dialog.ShowAsync();
        //        }
        //    }
        //}

        private async void ButtonLyricsSitesDataDirectory_Click(object sender, RoutedEventArgs e)
        {
            string directoryPath = Path.GetDirectoryName(SettingsHelper.LyricsSitesDataFilePath);
            string fileName = Path.GetFileName(SettingsHelper.LyricsSitesDataFilePath);

            var folder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
            if (folder != null)
            {
                var item = await folder.GetItemAsync(fileName);

                var folderLauncherOptions = new Windows.System.FolderLauncherOptions();
                folderLauncherOptions.ItemsToSelect.Add(item);

                await Windows.System.Launcher.LaunchFolderPathAsync(directoryPath, folderLauncherOptions);
            }
        }

        private void TextBoxLyricsSitesData_Loaded(object sender, RoutedEventArgs e)
        {
            string fileName = Path.GetFileName(SettingsHelper.LyricsSitesDataFilePath);
            textBoxLyricsSitesData.Text = fileName;
        }
    }
}
