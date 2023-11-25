using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
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
            RawTextCopiedTip.IsOpen = false;
            RawTextPastedTip.IsOpen = false;
            EncodedTextCopiedTip.IsOpen = false;
            EncodedTextPastedTip.IsOpen = false;
        }

        private async void SelectMode_Click(object sender, RoutedEventArgs e)
        {
            EncodeMode.Content = sender.GetPropertyValue<string>("Text");
            converter_mode = (CodecMode)Enum.Parse(typeof(CodecMode), sender.GetPropertyValue<string>("Name"));

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

            await StartCodecAsync();
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (last_focused_is_raw_text && sender.GetPropertyValue<string>("Name") == "RawTextBox" ||
                !last_focused_is_raw_text && sender.GetPropertyValue<string>("Name") == "EncodedTextBox")
                await StartCodecAsync();
        }

        private void RawTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_is_raw_text = true;
        }

        private void EncodedTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_is_raw_text = false;
        }

        private async void EncodeWithSpace_Changed(object sender, RoutedEventArgs e)
        {
            await StartCodecAsync();
        }

        private async void BaseSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await StartCodecAsync();
        }

        private async void ChineseTeleCodeStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await StartCodecAsync();
        }

        private async Task StartCodecAsync()
        {
            if (last_focused_is_raw_text)
            {
                var raw_text = RawTextBox.Text;
                var encoded_text = await Task.Run(() => GetEncodedText(raw_text));
                EncodedTextBox.Text = encoded_text;
            }
            else
            {
                var encoded_text = EncodedTextBox.Text;
                var decoded_text = await Task.Run(() => GetDecodedText(encoded_text));
                RawTextBox.Text = decoded_text;
            }
        }

        private string GetEncodedText(string raw_text)
        {
            return converter_mode switch
            {
                CodecMode.UnicodeBin => UnicodeCodec.BinEncoder(raw_text),
                CodecMode.UnicodeOct => UnicodeCodec.OctEncoder(raw_text),
                CodecMode.UnicodeDec => UnicodeCodec.DecEncoder(raw_text),
                CodecMode.UnicodeHex => UnicodeCodec.HexEncoder(raw_text),

                CodecMode.UTF8 => UtfCodec.Utf8Encoder(raw_text),
                CodecMode.UTF16LE => UtfCodec.Utf16LeEncoder(raw_text),
                CodecMode.UTF16BE => UtfCodec.Utf16BeEncoder(raw_text),

                CodecMode.Base64 => BaseSeriesCodec.Base64Encoder(raw_text),
                CodecMode.Base58 => BaseSeriesCodec.Base58Encoder(raw_text),
                CodecMode.Base32 => BaseSeriesCodec.Base32Encoder(raw_text),

                CodecMode.JsonString => JsonStringCodec.Encoder(raw_text),
                CodecMode.InternationalMorseCode => MorseCodeCodec.Encoder(raw_text),
                CodecMode.ChineseTelegraphCode => ChineseTelegraphCodec.Encoder(raw_text),

                _ => raw_text,
            };
        }

        private string GetDecodedText(string encoded_text)
        {
            return converter_mode switch
            {
                CodecMode.UnicodeBin => UnicodeCodec.BinDecoder(encoded_text),
                CodecMode.UnicodeOct => UnicodeCodec.OctDecoder(encoded_text),
                CodecMode.UnicodeDec => UnicodeCodec.DecDecoder(encoded_text),
                CodecMode.UnicodeHex => UnicodeCodec.HexDecoder(encoded_text),

                CodecMode.UTF8 => UtfCodec.Utf8Decoder(encoded_text),
                CodecMode.UTF16LE => UtfCodec.Utf16LeDecoder(encoded_text),
                CodecMode.UTF16BE => UtfCodec.Utf16BeDecoder(encoded_text),

                CodecMode.Base64 => BaseSeriesCodec.Base64Decoder(encoded_text),
                CodecMode.Base58 => BaseSeriesCodec.Base58Decoder(encoded_text),
                CodecMode.Base32 => BaseSeriesCodec.Base32Decoder(encoded_text),

                CodecMode.JsonString => JsonStringCodec.Decoder(encoded_text),
                CodecMode.InternationalMorseCode => MorseCodeCodec.Decoder(encoded_text),
                CodecMode.ChineseTelegraphCode => ChineseTelegraphCodec.Decoder(encoded_text),

                _ => encoded_text,
            };
        }
    }
}
