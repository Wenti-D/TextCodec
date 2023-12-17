using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using TextCodec.Core;
using TextCodec.Extensions;
using Windows.ApplicationModel;

namespace TextCodec.ViewModels;
#nullable enable

public sealed class SettingsViewModel : ObservableObject
{
    #region fields
    private List<string> sysBackdrops;
    private string? selectedBackdrop;
    private Version version = Package.Current.Id.Version.ToVersion();
    private AppSettings _appSettings;
    #endregion

    #region properties
    public List<string> SysBackdrops { get { return sysBackdrops; } }

    public string? SelectedBackdrop
    {
        get => selectedBackdrop ??= Enum.GetName(_appSettings.BackdropType);
        set
        {
            if (SetProperty(ref selectedBackdrop, value) && value != null)
            {
                _appSettings.BackdropType = (BackdropTypes)Enum.Parse(typeof(BackdropTypes), value);
            }
        }
    }

    public Version Version { get { return version; } }
    #endregion

    #region methods
    public SettingsViewModel(AppSettings appSettings)
    {
        sysBackdrops = new List<string>();
        _appSettings = appSettings;

        foreach (var backdrop_type in Enum.GetValues(typeof(BackdropTypes)))
        {
            sysBackdrops.Add(backdrop_type.ToString());
        }
    }
    #endregion
}
