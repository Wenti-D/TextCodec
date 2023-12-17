using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Security.Cryptography;
using System.Text;
using TextCodec.Core;
using TextCodec.Helpers;
using TextCodec.ViewModels;
using Vanara.Extensions.Reflection;
using Windows.ApplicationModel.DataTransfer;

namespace TextCodec.Views.Pages;

/// <summary>
/// …¢¡–º∆À„“≥√Ê
/// </summary>
public partial class HashPage : Page
{
    private readonly IServiceProvider serviceProvider;
    public HashViewModel ViewModel { get; }

    public HashPage()
    {
        serviceProvider = Ioc.Default;
        InitializeComponent();
        ViewModel = serviceProvider.GetRequiredService<HashViewModel>();
        DataContext = this;
    }
}
