using System;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using TextCodec.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace TextCodec.Views.Pages;

/// <summary>
/// ÂÒÂë»Ö¸´Ò³¡£
/// </summary>
public sealed partial class MojibakeRecoverPage : Page
{
    private readonly IServiceProvider serviceProvider;
    public MojibakeRecoverViewModel ViewModel { get; }

    public MojibakeRecoverPage()
    {
        serviceProvider = Ioc.Default;
        ViewModel = serviceProvider.GetRequiredService<MojibakeRecoverViewModel>();
        InitializeComponent();
        DataContext = this;
    }
}
