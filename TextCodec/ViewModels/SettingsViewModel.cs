using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TextCodec.Core;
using TextCodec.Extensions;
using Windows.ApplicationModel;

namespace TextCodec.ViewModels;
#nullable enable

public partial class SettingsViewModel : ObservableObject
{
    #region fields

    private readonly IServiceProvider serviceProvider;
    private readonly AppSettings appSettings;

    #endregion

    #region properties

    [ObservableProperty]
    private Version version;
    [ObservableProperty]
    private List<string> sysBackdrops;

    /// <summary>
    /// 选中的背景材质
    /// </summary>
    private string? selectedBackdrop;

    public string? SelectedBackdrop
    {
        get => selectedBackdrop ??= Enum.GetName(appSettings.BackdropType);
        set
        {
            if (SetProperty(ref selectedBackdrop, value) && value is not null)
            {
                appSettings.BackdropType = (BackdropTypes)Enum.Parse(typeof(BackdropTypes), value);
            }
        }
    }

    #endregion

    #region methods

    public SettingsViewModel()
    {
        serviceProvider = Ioc.Default;
        appSettings = serviceProvider.GetRequiredService<AppSettings>();

        version = Package.Current.Id.Version.ToVersion();

        sysBackdrops = new List<string>();
        foreach (var backdropType in Enum.GetValues(typeof(BackdropTypes)))
        {
            sysBackdrops.Add(backdropType.ToString()!);
        }
    }

    [RelayCommand]
    private void ResetSettings()
    {
        appSettings.OnReset = true;
    }

    #endregion
}
