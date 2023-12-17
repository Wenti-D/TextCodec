using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Windows.Storage;

namespace TextCodec.Core;
#nullable enable

public sealed class AppSettings : ObservableObject
{
    private BackdropTypes? backdropTypes;
    private bool? isMainWindowMaximum;
    private ulong? mainWindowRect;

    private bool? isUtfEncodeWithSpace;
    private string? baseSeriesTextPreprocessMode;
    private string? base58Style;
    private string? chineseTelegraphCodeStyle;

    private string? hashTextPreprocessMode;
    private string? hashMode;

    private bool? onReset;
    private ApplicationDataContainer local_settings = ApplicationData.Current.LocalSettings;

    /// <summary>
    /// 选中的背景材质
    /// </summary>
    public BackdropTypes BackdropType
    {
        get => backdropTypes ?? Enum.Parse<BackdropTypes>("Mica");
        set
        {
            SetProperty(ref backdropTypes, value);
            local_settings.Values["BackdropType"] = value.ToString();
        }
    }

    /// <summary>
    /// 窗口是否最大化
    /// </summary>
    public bool IsMainWindowMaximum
    {
        get => isMainWindowMaximum ?? false;
        set
        {
            SetProperty(ref isMainWindowMaximum, value);
            local_settings.Values["IsMainWindowMaximum"] = value;
        }
    }

    /// <summary>
    /// 主窗口位置大小
    /// </summary>
    public ulong MainWindowRect
    {
        get => mainWindowRect ?? 0;
        set
        {
            SetProperty(ref mainWindowRect, value);
            local_settings.Values["MainWindowRect"] = value;
        }
    }

    /// <summary>
    /// UTF「编码时加空格」设置
    /// </summary>
    public bool IsUtfEncodeWithSpace
    {
        get => isUtfEncodeWithSpace ?? true;
        set
        {
            SetProperty(ref isUtfEncodeWithSpace, value);
            local_settings.Values["IsUtfEncodeWithSpace"] = value;
        }
    }

    /// <summary>
    /// Base 系列文本预编码方式
    /// </summary>
    public string BaseSeriesTextPreprocessMode
    {
        get => baseSeriesTextPreprocessMode ?? "CodecPageModeUtf8";
        set
        {
            SetProperty(ref baseSeriesTextPreprocessMode, value);
            local_settings.Values["BaseSeriesTextPreprocessMode"] = value;
        }
    }

    /// <summary>
    /// Base 58 编码类型
    /// </summary>
    public string Base58Style
    {
        get => base58Style ?? "CodecPageBase58StdCharList";
        set
        {
            SetProperty(ref base58Style, value);
            local_settings.Values["Base58Style"] = value;
        }
    }

    /// <summary>
    /// 中文电码编码样式
    /// </summary>
    public string ChineseTelegraphCodeStyle
    {
        get => chineseTelegraphCodeStyle ?? "ChineseTeleCodeNumber";
        set
        {
            SetProperty(ref chineseTelegraphCodeStyle, value);
            local_settings.Values["ChineseTelegraphCodeStyle"] = value;
        }
    }

    /// <summary>
    /// 散列计算的文本预处理编码方式
    /// </summary>
    public string HashTextPreprocessMode
    {
        get => hashTextPreprocessMode ?? "HashPreprocessModeUtf8";
        set
        {
            SetProperty(ref hashTextPreprocessMode, value);
            local_settings.Values["HashTextPreprocessMode"] = value;
        }
    }

    /// <summary>
    /// 散列算法
    /// </summary>
    public string HashMode
    {
        get => hashMode ?? "MD5";
        set
        {
            SetProperty(ref hashMode, value);
            local_settings.Values["HashMode"] = value;
        }
    }

    /// <summary>
    /// 是否处于重置状态
    /// </summary>
    public bool OnReset
    {
        get => onReset ?? false;
        set
        {
            SetProperty(ref onReset, value);
            local_settings.Values["OnReset"] = value;
        }
    }

    public AppSettings()
    {
        Init();
        GetSettings();
    }

    private void Init()
    {
        if (local_settings.Values["IsInitialized"] is null)
        {
            local_settings.Values["IsInitialized"] = true;

            local_settings.Values["BackdropType"] = "Mica";
            local_settings.Values["IsMainWindowMaximum"] = false;

            local_settings.Values["IsUtfEncodeWithSpace"] = true;
            local_settings.Values["BaseSeriesTextPreprocessMode"] = "CodecPageModeUtf8";
            local_settings.Values["Base58Style"] = "CodecPageBase58StdCharList";

            local_settings.Values["HashTextPreprocessMode"] = "HashPreprocessModeUtf8";
            local_settings.Values["HashMode"] = "MD5";

            local_settings.Values["OnReset"] = false;
        }
    }

    private void GetSettings()
    {
        backdropTypes = Enum.Parse<BackdropTypes>(local_settings.Values["BackdropType"] as string);
        isMainWindowMaximum = (bool?)local_settings.Values["IsMainWindowMaximum"];
        mainWindowRect = (ulong?)local_settings.Values["MainWindowRect"];

        isUtfEncodeWithSpace = (bool?)local_settings.Values["IsUtfEncodeWithSpace"];
        baseSeriesTextPreprocessMode = local_settings.Values["BaseSeriesTextPreprocessMode"] as string;
        base58Style = local_settings.Values["Base58Style"] as string;

        hashTextPreprocessMode = local_settings.Values["HashTextPreprocessMode"] as string;
        hashMode = local_settings.Values["HashMode"] as string;

        onReset = (bool?)local_settings.Values["OnReset"];
    }
}
