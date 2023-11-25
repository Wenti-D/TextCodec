using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using TextCodec.Core;
using Vanara.Extensions.Reflection;
using Windows.ApplicationModel.DataTransfer;

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
            EncodedTextBox.AddHandler(PointerPressedEvent, new PointerEventHandler(EncodedTextBox_PointerPressed), true);
            RawTextBox.AddHandler(PointerPressedEvent, new PointerEventHandler(RawTextBox_PointerPressed), true);

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(600);
        }

        private void ClearText_Click(object sender, RoutedEventArgs e)
        {
            RawTextBox.Text = EncodedTextBox.Text = string.Empty;
        }

        private void CopyRawText_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            package.SetText(RawTextBox.Text);
            Clipboard.SetContent(package);
            RawTextCopiedTip.IsOpen = true;
            timer.Start();
        }

        private async void PasteRawText_Click(object sender, RoutedEventArgs e)
        {
            last_focused_is_raw_text = true;
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                RawTextBox.Text = await package.GetTextAsync();
            }
            RawTextPastedTip.IsOpen = true;
            timer.Start();
        }

        private void CopyEncodedText_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            package.SetText(EncodedTextBox.Text);
            Clipboard.SetContent(package);
            EncodedTextCopiedTip.IsOpen = true;
            timer.Start();
        }

        private async void PasteEncodedText_Click(object sender, RoutedEventArgs e)
        {
            last_focused_is_raw_text = false;
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                EncodedTextBox.Text = await package.GetTextAsync();
            }
            EncodedTextPastedTip.IsOpen = true;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            (sender as DispatcherTimer).Stop();
            if (RawTextCopiedTip.IsOpen) RawTextCopiedTip.IsOpen = false;
            if (RawTextPastedTip.IsOpen) RawTextPastedTip.IsOpen = false;
            if (EncodedTextCopiedTip.IsOpen) EncodedTextCopiedTip.IsOpen = false;
            if (EncodedTextPastedTip.IsOpen) EncodedTextPastedTip.IsOpen = false;
        }

        private void SelectMode_Click(object sender, RoutedEventArgs e)
        {
            string newMode = sender.GetPropertyValue<string>("Text");
            string newConvertMode = sender.GetPropertyValue<string>("Name");
            EncodeMode.Content = newMode;
            converter_mode = (CodecMode)Enum.Parse(typeof(CodecMode), newConvertMode);
            if (converter_mode >= (CodecMode)Enum.Parse(typeof(CodecMode), "UTF8")
                && converter_mode <= (CodecMode)Enum.Parse(typeof(CodecMode), "UTF16BE"))
                EncodeWithSpace.Visibility = Visibility.Visible;
            else
                EncodeWithSpace.Visibility = Visibility.Collapsed;
            if (converter_mode >= (CodecMode)Enum.Parse(typeof(CodecMode), "Base64")
                && converter_mode <= (CodecMode)Enum.Parse(typeof(CodecMode), "Base32"))
                TextPreprocessMode.Visibility = Visibility.Visible;
            else
                TextPreprocessMode.Visibility = Visibility.Collapsed;
            if (converter_mode == (CodecMode)Enum.Parse(typeof(CodecMode), "Base58"))
                Base58Style.Visibility = Visibility.Visible;
            else
                Base58Style.Visibility = Visibility.Collapsed;
            if (converter_mode == (CodecMode)Enum.Parse(typeof(CodecMode), "ChineseTelegraphCode"))
                ChineseTeleCodeStyle.Visibility = Visibility.Visible;
            else
                ChineseTeleCodeStyle.Visibility = Visibility.Collapsed;
            StartCodec();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (last_focused_is_raw_text && sender.GetPropertyValue<string>("Name") == "RawTextBox" ||
                !last_focused_is_raw_text && sender.GetPropertyValue<string>("Name") == "EncodedTextBox")
                StartCodec();
        }

        private void RawTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_is_raw_text = true;
        }

        private void EncodedTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_is_raw_text = false;
        }

        private void StartCodec()
        {
            if (last_focused_is_raw_text)
            {
                EncodedTextBox.Text = converter_mode switch
                {
                    CodecMode.UnicodeBin => UnicodeCodec.BinEncoder(RawTextBox.Text),
                    CodecMode.UnicodeOct => UnicodeCodec.OctEncoder(RawTextBox.Text),
                    CodecMode.UnicodeDec => UnicodeCodec.DecEncoder(RawTextBox.Text),
                    CodecMode.UnicodeHex => UnicodeCodec.HexEncoder(RawTextBox.Text),

                    CodecMode.UTF8 => UtfCodec.Utf8Encoder(RawTextBox.Text),
                    CodecMode.UTF16LE => UtfCodec.Utf16LeEncoder(RawTextBox.Text),
                    CodecMode.UTF16BE => UtfCodec.Utf16BeEncoder(RawTextBox.Text),

                    CodecMode.Base64 => BaseSeriesCodec.Base64Encoder(RawTextBox.Text),
                    CodecMode.Base58 => BaseSeriesCodec.Base58Encoder(RawTextBox.Text),
                    CodecMode.Base32 => BaseSeriesCodec.Base32Encoder(RawTextBox.Text),

                    CodecMode.JsonString => JsonStringCodec.Encoder(RawTextBox.Text),
                    CodecMode.InternationalMorseCode => MorseCodeCodec.Encoder(RawTextBox.Text),
                    CodecMode.ChineseTelegraphCode => ChineseTelegraphCodec.Encoder(RawTextBox.Text),

                    _ => RawTextBox.Text,
                };
            }
            else
            {
                RawTextBox.Text = converter_mode switch
                {
                    CodecMode.UnicodeBin => UnicodeCodec.BinDecoder(EncodedTextBox.Text),
                    CodecMode.UnicodeOct => UnicodeCodec.OctDecoder(EncodedTextBox.Text),
                    CodecMode.UnicodeDec => UnicodeCodec.DecDecoder(EncodedTextBox.Text),
                    CodecMode.UnicodeHex => UnicodeCodec.HexDecoder(EncodedTextBox.Text),

                    CodecMode.UTF8 => UtfCodec.Utf8Decoder(EncodedTextBox.Text),
                    CodecMode.UTF16LE => UtfCodec.Utf16LeDecoder(EncodedTextBox.Text),
                    CodecMode.UTF16BE => UtfCodec.Utf16BeDecoder(EncodedTextBox.Text),

                    CodecMode.Base64 => BaseSeriesCodec.Base64Decoder(EncodedTextBox.Text),
                    CodecMode.Base58 => BaseSeriesCodec.Base58Decoder(EncodedTextBox.Text),
                    CodecMode.Base32 => BaseSeriesCodec.Base32Decoder(EncodedTextBox.Text),

                    CodecMode.JsonString => JsonStringCodec.Decoder(EncodedTextBox.Text),
                    CodecMode.InternationalMorseCode => MorseCodeCodec.Decoder(EncodedTextBox.Text),
                    CodecMode.ChineseTelegraphCode => ChineseTelegraphCodec.Decoder(EncodedTextBox.Text),

                    _ => EncodedTextBox.Text,
                };
            }

        }

        private void EncodeWithSpace_Changed(object sender, RoutedEventArgs e)
        {
            StartCodec();
        }

        private void BaseSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartCodec();
        }

        private void ChineseTeleCodeStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartCodec();
        }
    }
}
