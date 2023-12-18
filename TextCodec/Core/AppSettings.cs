using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Windows.Storage;

namespace TextCodec.Core;
#nullable enable

public partial class AppSettings : ObservableObject
{
    #region fields
    private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    #endregion

    #region properties

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

    /// <summary>
    /// 选中的背景材质
    /// </summary>
    public BackdropTypes BackdropType
    {
        get => backdropTypes ?? Enum.Parse<BackdropTypes>("Mica");
        set
        {
            SetProperty(ref backdropTypes, value);
            localSettings.Values["BackdropType"] = value.ToString();
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
            localSettings.Values["IsMainWindowMaximum"] = value;
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
            localSettings.Values["MainWindowRect"] = value;
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
            localSettings.Values["IsUtfEncodeWithSpace"] = value;
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
            localSettings.Values["BaseSeriesTextPreprocessMode"] = value;
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
            localSettings.Values["Base58Style"] = value;
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
            localSettings.Values["ChineseTelegraphCodeStyle"] = value;
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
            localSettings.Values["HashTextPreprocessMode"] = value;
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
            localSettings.Values["HashMode"] = value;
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
            localSettings.Values["OnReset"] = value;
        }
    }

    #endregion

    #region methods

    public AppSettings()
    {
        Init();
        GetSettings();
    }

    private void Init()
    {
        if (localSettings.Values["IsInitialized"] is null or false)
        {
            localSettings.Values["IsInitialized"] = true;

            localSettings.Values["BackdropType"] = "Mica";
            localSettings.Values["IsMainWindowMaximum"] = false;

            localSettings.Values["IsUtfEncodeWithSpace"] = true;
            localSettings.Values["BaseSeriesTextPreprocessMode"] = "CodecPageModeUtf8";
            localSettings.Values["Base58Style"] = "CodecPageBase58StdCharList";

            localSettings.Values["HashTextPreprocessMode"] = "HashPreprocessModeUtf8";
            localSettings.Values["HashMode"] = "MD5";

            localSettings.Values["OnReset"] = false;
        }
    }

    private void GetSettings()
    {
        backdropTypes = Enum.Parse<BackdropTypes>((string)localSettings.Values["BackdropType"]);
        isMainWindowMaximum = (bool)localSettings.Values["IsMainWindowMaximum"];
        mainWindowRect = (ulong?)localSettings.Values["MainWindowRect"];

        isUtfEncodeWithSpace = (bool)localSettings.Values["IsUtfEncodeWithSpace"];
        baseSeriesTextPreprocessMode = localSettings.Values["BaseSeriesTextPreprocessMode"] as string;
        base58Style = localSettings.Values["Base58Style"] as string;

        hashTextPreprocessMode = localSettings.Values["HashTextPreprocessMode"] as string;
        hashMode = localSettings.Values["HashMode"] as string;

        onReset = (bool)localSettings.Values["OnReset"];
    }

    #endregion
}
