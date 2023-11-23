using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
