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
        private bool last_focused_textbox;

        public CodecPage()
        {
            this.InitializeComponent();

            last_focused_textbox = false;
            encodedTextBox.AddHandler(PointerPressedEvent, new PointerEventHandler(encodedTextBox_PointerPressed), true);
            rawTextBox.AddHandler(PointerPressedEvent, new PointerEventHandler(rawTextBox_PointerPressed), true);
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }

        private void clearText_Click(object sender, RoutedEventArgs e)
        {
            rawTextBox.Text = null;
            encodedTextBox.Text = null;
        }

        private void unicodeBin_Click(object sender, RoutedEventArgs e)
        {
            encodeMode.Content = unicodeBin.Text;
        }

        private void unicodeOct_Click(object sender, RoutedEventArgs e)
        {
            encodeMode.Content = unicodeOct.Text;
        }

        private void unicodeDec_Click(object sender, RoutedEventArgs e)
        {
            encodeMode.Content = unicodeDec.Text;
        }

        private void unicodeHex_Click(object sender, RoutedEventArgs e)
        {
            encodeMode.Content = unicodeHex.Text;
        }

        private void rawTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (last_focused_textbox == false)
            {
                var unicodeHexCodec = new UnicodeHexCodec();
                String encodedText = unicodeHexCodec.hexEncoder(rawTextBox.Text);
                encodedTextBox.Text = encodedText;
            }
        }

        private void rawTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_textbox = false;
        }

        private void encodedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (last_focused_textbox == true)
            {
                var unicodeHexCodec = new UnicodeHexCodec();
                String rawText = unicodeHexCodec.hexDecoder(encodedTextBox.Text);
                rawTextBox.Text = rawText;
            }
        }

        private void encodedTextBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            last_focused_textbox = true;
        }
    }
}
