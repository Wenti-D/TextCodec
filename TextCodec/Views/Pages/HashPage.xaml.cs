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
using Windows.ApplicationModel.DataTransfer;
using Vanara.Extensions.Reflection;
using System.Security.Cryptography;
using System.Text;
using TextCodec.Helpers;
using Vanara.PInvoke;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TextCodec.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HashPage : Page
    {
        private HashMode hash_mode;
        private static DispatcherTimer timer;
        private static AppSettings AppSettings = MainWindow.AppSettings;

        public HashPage()
        {
            InitializeComponent();

            hash_mode = (HashMode)Enum.Parse(typeof(HashMode), "MD5");

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(600);
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

        private void hashMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string new_hash_mode = sender.GetPropertyValue<string>("SelectedValue");
            hash_mode = (HashMode)Enum.Parse(typeof(HashMode), new_hash_mode);
            StartHash();
        }

        private void textPreprocessMode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            StartHash();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartHash();
        }

        private void Timer_Tick(object sender, object e)
        {
            (sender as DispatcherTimer).Stop();
            if (rawTextCopiedTip.IsOpen) rawTextCopiedTip.IsOpen = false;
            if (rawTextPastedTip.IsOpen) rawTextPastedTip.IsOpen = false;
            if (encodedTextCopiedTip.IsOpen) encodedTextCopiedTip.IsOpen = false;
        }

        private void StartHash()
        {
            HashAlgorithm algorithm = hash_mode switch
            {
                HashMode.MD5 => MD5.Create(),
                HashMode.SHA1 => SHA1.Create(),
                HashMode.SHA256 => SHA256.Create(),
                HashMode.SHA512 => SHA512.Create(),
            };
            byte[] hash = algorithm.ComputeHash(new HashHelper().TextToBytes(rawTextBox.Text));
            StringBuilder str_builder = new();
            foreach (byte b in hash)
            {
                str_builder.AppendFormat("{0:X2}", b);
            }
            encodedTextBox.Text = str_builder.ToString();
        }
    }
}
