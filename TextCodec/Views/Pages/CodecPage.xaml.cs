using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using TextCodec.ViewModels;

namespace TextCodec.Views.Pages;

/// <summary>
/// ±‡Ω‚¬Î“≥√Ê
/// </summary>
public partial class CodecPage : Page
{
    private readonly IServiceProvider serviceProvider;
    public CodecViewModel ViewModel { get; }

    public CodecPage()
    {
        serviceProvider = Ioc.Default;
        InitializeComponent();
        ViewModel = serviceProvider.GetRequiredService<CodecViewModel>();
        DataContext = this;
    }
}
