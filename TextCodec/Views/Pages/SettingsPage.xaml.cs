using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using TextCodec.ViewModels;

namespace TextCodec.Views.Pages;

/// <summary>
/// …Ë÷√“≥
/// </summary>
public sealed partial class SettingsPage : Page
{
    private readonly IServiceProvider serviceProvider;
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        serviceProvider = Ioc.Default;
        ViewModel = serviceProvider.GetService<SettingsViewModel>();
        DataContext = this;

        InitializeComponent();
    }
}
