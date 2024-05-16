using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TextCodec.Core;
using TextCodec.Extensions;
using Vanara.Extensions.Reflection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;

namespace TextCodec.ViewModels;
#nullable enable

public partial class SettingsViewModel : ObservableObject
{
    #region fields

    private readonly IServiceProvider serviceProvider;
    private readonly AppSettings appSettings;
    private readonly ResourceLoader resourceLoader;

    #endregion

    #region properties

    [ObservableProperty]
    private Version version;
    [ObservableProperty]
    private List<string> sysBackdrops;
    [ObservableProperty]
    private CodecMode selectedDefaultCodecMode;
    [ObservableProperty]
    private string? selectedDefaultCodecModeText;
    [ObservableProperty]
    private HashMode selectedDefaultHashMode;

    private string? selectedBackdrop;
    private bool? isKeepLastModeEnabled;
    private string? selectedDefaultCodecModeName;
    private string? selectedDefaultHashModeText;
    private bool isInitialized;

    /// <summary>
    /// 选中的背景材质
    /// </summary>
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
    
    /// <summary>
    /// 「记住最后的模式」是否启用
    /// </summary>
    public bool IsKeepLastModeEnabled
    {
        get => isKeepLastModeEnabled ??= false;
        set
        {
            SetProperty(ref isKeepLastModeEnabled, value);
            appSettings.IsKeepLastModeEnabled = value;
        }
    }

    /// <summary>
    /// 默认文本编解码模式
    /// </summary>
    public string SelectedDefaultCodecModeName
    {
        get => selectedDefaultCodecModeName ??= appSettings.DefaultCodecMode;
        set
        {
            SetProperty(ref selectedDefaultCodecModeName, value);
            appSettings.DefaultCodecMode = value;
        }
    }

    /// <summary>
    /// 默认散列算法
    /// </summary>
    public string SelectedDefaultHashModeText
    {
        get => selectedDefaultHashModeText ??= appSettings.DefaultHashMode;
        set
        {
            SetProperty(ref selectedDefaultHashModeText, value);
            appSettings.DefaultHashMode = value;
        }
    }

    /// <summary>
    /// 是否处于初始化状态
    /// </summary>
    public bool IsInitialized
    {
        get => isInitialized;
        set
        {
            SetProperty(ref isInitialized, value);
            appSettings.IsInitialized = value;
        }
    }

    #endregion

    #region methods

    public SettingsViewModel()
    {
        serviceProvider = Ioc.Default;
        appSettings = serviceProvider.GetRequiredService<AppSettings>();
        resourceLoader = serviceProvider.GetRequiredService<ResourceLoader>();

        version = Package.Current.Id.Version.ToVersion();
        isKeepLastModeEnabled = appSettings.IsKeepLastModeEnabled;
        selectedDefaultCodecModeName = appSettings.DefaultCodecMode;
        selectedDefaultCodecModeText = GetTranslation(appSettings.DefaultCodecMode);
        selectedDefaultHashModeText = appSettings.DefaultHashMode;

        isInitialized = appSettings.IsInitialized;

        sysBackdrops = new List<string>();
        foreach (var backdropType in Enum.GetValues(typeof(BackdropTypes)))
        {
            sysBackdrops.Add(backdropType.ToString()!);
        }
    }

    private string GetTranslation(string resource)
    {
        string translation = resourceLoader.GetString(resource);
        if (translation == string.Empty)
        {
            return resource;
        }
        return translation;
    }

    [RelayCommand]
    private void SelectDefaultCodecMode(object? sender)
    {
        SelectedDefaultCodecModeText = sender?.GetPropertyValue<string>("Text");
        SelectedDefaultCodecModeName = sender?.GetPropertyValue<string>("Name") ?? "None";
        SelectedDefaultCodecMode = (CodecMode)Enum.Parse(typeof(CodecMode), SelectedDefaultCodecModeName);
    }

    public void SelectDefaultHashMode()
    {
        SelectedDefaultHashMode = (HashMode)Enum.Parse(typeof(HashMode), SelectedDefaultHashModeText);
    }

    [RelayCommand]
    private void ResetSettings()
    {
        IsInitialized = false;
    }

    #endregion
}
