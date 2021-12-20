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

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace MusicLyrics
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
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
            }
        }
    }
}
