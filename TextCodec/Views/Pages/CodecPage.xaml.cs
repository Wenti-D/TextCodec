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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TextCodec.Views.Pages
{
    /// <summary>
    /// 编解码页面
    /// </summary>
    public sealed partial class CodecPage : Page
    {
        // 最后交互的文本框：f-原文；t-编码文本
        private bool last_focused_is_raw_text;
        private ConverterModes.ConverterMode converter_mode;

        public CodecPage()
        {
            InitializeComponent();

            last_focused_is_raw_text = false;
            converter_mode = (ConverterModes.ConverterMode)Enum.Parse(typeof(ConverterModes.ConverterMode), "None");
            encodedTextBox.AddHandler(PointerPressedEvent, new PointerEventHandler(encodedTextBox_PointerPressed), true);
            rawTextBox.AddHandler(PointerPressedEvent, new PointerEventHandler(rawTextBox_PointerPressed), true);
        }

        private void ClearText_Click(object sender, RoutedEventArgs e)
        {
            rawTextBox.Text = encodedTextBox.Text = string.Empty;
        }

        private void SelectMode_Click(object sender, RoutedEventArgs e)
        {
            string newMode = sender.GetPropertyValue<string>("Text");
            string newConvertMode = sender.GetPropertyValue<string>("Name");
            encodeMode.Content = newMode;
            converter_mode = (ConverterModes.ConverterMode)Enum.Parse(typeof(ConverterModes.ConverterMode), newConvertMode);
            StartCodec();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartCodec();
        }

        private void rawTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_is_raw_text = false;
        }

        private void encodedTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_is_raw_text = true;
        }

        private void StartCodec()
        {
            if (last_focused_is_raw_text == false)
            {
                switch (converter_mode)
                {
                    case ConverterModes.ConverterMode.UnicodeBin:
                        encodedTextBox.Text = UnicodeCodec.BinEncoder(rawTextBox.Text);
                        break;
                    case ConverterModes.ConverterMode.UnicodeOct:
                        encodedTextBox.Text = UnicodeCodec.OctEncoder(rawTextBox.Text);
                        break;
                    case ConverterModes.ConverterMode.UnicodeDec:
                        encodedTextBox.Text = UnicodeCodec.DecEncoder(rawTextBox.Text);
                        break;
                    case ConverterModes.ConverterMode.UnicodeHex:
                        encodedTextBox.Text = UnicodeCodec.HexEncoder(rawTextBox.Text);
                        break;
                    default:
                        encodedTextBox.Text = rawTextBox.Text;
                        break;
                }
            }
            else
            {
                switch (converter_mode)
                {
                    case ConverterModes.ConverterMode.UnicodeBin:
                        rawTextBox.Text = UnicodeCodec.BinDecoder(encodedTextBox.Text);
                        break;
                    case ConverterModes.ConverterMode.UnicodeOct:
                        rawTextBox.Text = UnicodeCodec.OctDecoder(encodedTextBox.Text);
                        break;
                    case ConverterModes.ConverterMode.UnicodeDec:
                        rawTextBox.Text = UnicodeCodec.DecDecoder(encodedTextBox.Text);
                        break;
                    case ConverterModes.ConverterMode.UnicodeHex:
                        rawTextBox.Text = UnicodeCodec.HexDecoder(encodedTextBox.Text);
                        break;
                    default:
                        rawTextBox.Text = encodedTextBox.Text;
                        break;
                }
            }

        }
    }
}
