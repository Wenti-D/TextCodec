using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using TextCodec.Core;
using Vanara.Extensions.Reflection;
using Windows.ApplicationModel.DataTransfer;
using System.Timers;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TextCodec.Views.Pages
{
    /// <summary>
    /// 编解码页面
    /// </summary>
    public sealed partial class CodecPage : Page
    {
        // 最后交互的文本框：T-原文；F-编码文本
        private bool last_focused_is_raw_text;
        private CodecMode converter_mode;
        private static DispatcherTimer timer;

        public CodecPage()
        {
            InitializeComponent();

            last_focused_is_raw_text = true;
            converter_mode = (CodecMode)Enum.Parse(typeof(CodecMode), "None");
            encodedTextBox.AddHandler(PointerPressedEvent, new PointerEventHandler(encodedTextBox_PointerPressed), true);
            rawTextBox.AddHandler(PointerPressedEvent, new PointerEventHandler(rawTextBox_PointerPressed), true);

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(600);

            if (ApplicationData.Current.LocalSettings.Values["IsUtfEncodeWithSpaceChecked"] is null)
            {
                ApplicationData.Current.LocalSettings.Values["IsUtfEncodeWithSpaceChecked"] = true;
            }
            encodeWithSpace.IsChecked = (bool)ApplicationData.Current.LocalSettings.Values["IsUtfEncodeWithSpaceChecked"];
        }

        private void ClearText_Click(object sender, RoutedEventArgs e)
        {
            rawTextBox.Text = encodedTextBox.Text = string.Empty;
        }

        private void copyRawText_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            package.SetText(rawTextBox.Text);
            Clipboard.SetContent(package);
            rawTextCopiedTip.IsOpen = true;
            timer.Start();
        }

        private async void pasteRawText_Click(object sender, RoutedEventArgs e)
        {
            last_focused_is_raw_text = true;
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                rawTextBox.Text = await package.GetTextAsync();
            }
            rawTextPastedTip.IsOpen = true;
            timer.Start();
        }

        private void copyEncodedText_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            package.SetText(encodedTextBox.Text);
            Clipboard.SetContent(package);
            encodedTextCopiedTip.IsOpen = true;
            timer.Start();
        }

        private async void pasteEncodedText_Click(object sender, RoutedEventArgs e)
        {
            last_focused_is_raw_text = false;
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                encodedTextBox.Text = await package.GetTextAsync();
            }
            encodedTextPastedTip.IsOpen = true;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            (sender as DispatcherTimer).Stop();
            if (rawTextCopiedTip.IsOpen) rawTextCopiedTip.IsOpen = false;
            if (rawTextPastedTip.IsOpen) rawTextPastedTip.IsOpen = false;
            if (encodedTextCopiedTip.IsOpen) encodedTextCopiedTip.IsOpen = false;
            if (encodedTextPastedTip.IsOpen) encodedTextPastedTip.IsOpen = false;
        }

        private void SelectMode_Click(object sender, RoutedEventArgs e)
        {
            string newMode = sender.GetPropertyValue<string>("Text");
            string newConvertMode = sender.GetPropertyValue<string>("Name");
            encodeMode.Content = newMode;
            converter_mode = (CodecMode)Enum.Parse(typeof(CodecMode), newConvertMode);
            if (converter_mode >= (CodecMode)Enum.Parse(typeof(CodecMode), "UTF8")
                && converter_mode <= (CodecMode)Enum.Parse(typeof(CodecMode), "UTF16BE"))
                encodeWithSpace.Visibility = Visibility.Visible;
            else
                encodeWithSpace.Visibility = Visibility.Collapsed;
            if (converter_mode >= (CodecMode)Enum.Parse(typeof(CodecMode), "Base64")
                && converter_mode <= (CodecMode)Enum.Parse(typeof(CodecMode), "Base58"))
                textPreprocessMode.Visibility = Visibility.Visible;
            else
                textPreprocessMode.Visibility = Visibility.Collapsed;
            if (converter_mode == (CodecMode)Enum.Parse(typeof(CodecMode), "Base58"))
                Base58Style.Visibility = Visibility.Visible;
            else
                Base58Style.Visibility = Visibility.Collapsed;
            StartCodec();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (last_focused_is_raw_text && sender.GetPropertyValue<string>("Name") == "rawTextBox" ||
                !last_focused_is_raw_text && sender.GetPropertyValue<string>("Name") == "encodedTextBox")
                StartCodec();
        }

        private void rawTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_is_raw_text = true;
        }

        private void encodedTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_is_raw_text = false;
        }

        private void StartCodec()
        {
            if (last_focused_is_raw_text)
            {
                encodedTextBox.Text = converter_mode switch
                {
                    CodecMode.UnicodeBin => UnicodeCodec.BinEncoder(rawTextBox.Text),
                    CodecMode.UnicodeOct => UnicodeCodec.OctEncoder(rawTextBox.Text),
                    CodecMode.UnicodeDec => UnicodeCodec.DecEncoder(rawTextBox.Text),
                    CodecMode.UnicodeHex => UnicodeCodec.HexEncoder(rawTextBox.Text),

                    CodecMode.UTF8 => UtfCodec.Utf8Encoder(rawTextBox.Text),
                    CodecMode.UTF16LE => UtfCodec.Utf16LeEncoder(rawTextBox.Text),
                    CodecMode.UTF16BE => UtfCodec.Utf16BeEncoder(rawTextBox.Text),

                    CodecMode.Base64 => BaseSeriesCodec.Base64Encoder(rawTextBox.Text),
                    CodecMode.Base58 => BaseSeriesCodec.Base58Encoder(rawTextBox.Text),

                    _ => rawTextBox.Text,
                };
            }
            else
            {
                rawTextBox.Text = converter_mode switch
                {
                    CodecMode.UnicodeBin => UnicodeCodec.BinDecoder(encodedTextBox.Text),
                    CodecMode.UnicodeOct => UnicodeCodec.OctDecoder(encodedTextBox.Text),
                    CodecMode.UnicodeDec => UnicodeCodec.DecDecoder(encodedTextBox.Text),
                    CodecMode.UnicodeHex => UnicodeCodec.HexDecoder(encodedTextBox.Text),

                    CodecMode.UTF8 => UtfCodec.Utf8Decoder(encodedTextBox.Text),
                    CodecMode.UTF16LE => UtfCodec.Utf16LeDecoder(encodedTextBox.Text),
                    CodecMode.UTF16BE => UtfCodec.Utf16BeDecoder(encodedTextBox.Text),

                    CodecMode.Base64 => BaseSeriesCodec.Base64Decoder(encodedTextBox.Text),

                    _ => encodedTextBox.Text,
                };
            }

        }

        private void encodeWithSpace_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["IsUtfEncodeWithSpaceChecked"] = true;
            StartCodec();
        }

        private void encodeWithSpace_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["IsUtfEncodeWithSpaceChecked"] = false;
            StartCodec();
        }
    }
}
