using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using TextCodec.Core;
using TextCodec.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TextCodec.Views.Pages
{
    /// <summary>
    /// …Ë÷√“≥
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private IServiceProvider serviceProvider;
        private readonly AppSettings _appSettings;
        public SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            serviceProvider = Ioc.Default;
            _appSettings = serviceProvider.GetRequiredService<AppSettings>();
            ViewModel = serviceProvider.GetService<SettingsViewModel>();
            DataContext = ViewModel;

            InitializeComponent();
        }

        private void SettingsPageResetAppCard_Click(object sender, RoutedEventArgs e)
        {
            _appSettings.OnReset = true;
        }
    }
}
