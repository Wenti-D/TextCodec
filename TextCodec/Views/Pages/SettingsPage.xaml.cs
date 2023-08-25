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
    /// …Ë÷√“≥
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        AppSettings AppSettings = MainWindow.AppSettings;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void SettingsPageResetAppCard_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.OnReset = true;
        }
    }
}
