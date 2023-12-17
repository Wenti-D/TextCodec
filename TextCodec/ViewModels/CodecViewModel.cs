using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using TextCodec.Core;
using Vanara.Extensions.Reflection;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Globalization.NumberFormatting;

namespace TextCodec.ViewModels;
#nullable enable

public partial class CodecViewModel : ObservableObject
{
    #region fields

    private readonly AppSettings appSettings;
    private readonly IServiceProvider serviceProvider;
    private readonly ResourceLoader resourceLoader;
    private readonly UtfCodec utfCodec;
    private readonly BaseSeriesCodec baseSeriesCodec;
    private readonly ChineseTelegraphCodec chineseTelegraphCodec;
    private readonly DispatcherTimer timer;
    private TaskCompletionSource<bool>? currentCaesarShiftChanged;

    #endregion

    #region properties

    [ObservableProperty] // 原始文本
    private string? rawText;
    [ObservableProperty] // 编码后文本
    private string? encodedText;
    [ObservableProperty] // 最后交互的文本框是否为原始文本框，用于标记模式
    private bool lastFocusedIsRawTextBox;
    [ObservableProperty] // 原始文本的「复制成功」提示是否打开
    private bool isRawTextCopiedTipOpen;
    [ObservableProperty] // 原始文本的「粘贴成功」提示是否打开
    private bool isRawTextPastedTipOpen;
    [ObservableProperty] // 编码后文本的「复制成功」提示是否打开
    private bool isEncodedTextCopiedTipOpen;
    [ObservableProperty] // 编码后文本的「粘贴成功」提示是否打开
    private bool isEncodedTextPastedTipOpen;
    [ObservableProperty] // 当前编解码模式
    private CodecMode currentCodecMode;
    [ObservableProperty] // 当前编解码模式名称文本
    private string currentCodecModeText;
    [ObservableProperty] // 「UTF 编码时带空格」是否可见
    private bool isEncodeWithSpaceVisible;
    [ObservableProperty] // 「UTF 文本预处理模式」是否可见
    private bool isTextPreprocessModeVisible;
    [ObservableProperty] // 「Base58 样式」是否可见
    private bool isBase58StyleVisible;
    [ObservableProperty] // 「中文电码样式」是否可见
    private bool isChineseTeleCodeStyleVisible;
    [ObservableProperty] // 「凯撒密码偏移量」是否可见
    private bool isCaesarCipherShiftVisible;
    [ObservableProperty] // 「凯撒密码偏移量」内容格式化器
    private DecimalFormatter caesarCipherNumberFormatter;

    private bool isEncodeWithSpaceChecked;
    private string selectedBaseSeriesTextPreprocessMode;
    private string selectedBase58Style;
    private string selectedChineseTelegraphCodeStyle;
    private int currentCaesarShift;
    private string currentCaesarShiftText;

    /// <summary>
    /// UTF 系列「编码时带空格」是否勾选
    /// </summary>
    public bool IsEncodeWithSpaceChecked
    {
        get { return isEncodeWithSpaceChecked; }
        set
        {
            SetProperty(ref isEncodeWithSpaceChecked, value);
            appSettings.IsUtfEncodeWithSpace = value;
        }
    }

    /// <summary>
    /// 已选的 Base 系列文本预编码模式
    /// </summary>
    public string SelectedBaseSeriesTextPreprocessMode
    {
        get { return selectedBaseSeriesTextPreprocessMode; }
        set
        {
            SetProperty(ref selectedBaseSeriesTextPreprocessMode, value);
            appSettings.BaseSeriesTextPreprocessMode = value;
        }
    }

    /// <summary>
    /// 已选的 Base58 样式
    /// </summary>
    public string SelectedBase58Style
    {
        get { return selectedBase58Style; }
        set
        {
            SetProperty(ref selectedBase58Style, value);
            appSettings.Base58Style = value;
        }
    }

    /// <summary>
    /// 中文电码编码样式
    /// </summary>
    public string SelectedChineseTelegraphCodeStyle
    {
        get { return selectedChineseTelegraphCodeStyle; }
        set
        {
            SetProperty(ref selectedChineseTelegraphCodeStyle, value);
            appSettings.ChineseTelegraphCodeStyle = value;
        }
    }

    /// <summary>
    /// 当前凯撒密码偏移值
    /// </summary>
    public int CurrentCaesarShift
    {
        get => currentCaesarShift;
        set
        {
            SetProperty(ref currentCaesarShift, value);
            currentCaesarShiftChanged?.TrySetResult(true);
        }
    }

    /// <summary>
    /// 当前的凯撒密码偏移值文本
    /// </summary>
    public string CurrentCaesarShiftText
    {
        get { return currentCaesarShiftText; }
        set
        {
            SetProperty(ref currentCaesarShiftText, value);
            try
            {
                CurrentCaesarShift = int.Parse(value);
            }
            catch (Exception)
            {
                CurrentCaesarShift = 0;
            }
        }
    }

    #endregion

    #region methods

    public CodecViewModel()
    {
        // 初始化服务与程序设定
        serviceProvider = Ioc.Default;
        appSettings = serviceProvider.GetRequiredService<AppSettings>();
        resourceLoader = serviceProvider.GetRequiredService<ResourceLoader>();

        // 获取部分编解码器实例（静态的不用获取）
        utfCodec = serviceProvider.GetRequiredService<UtfCodec>();
        baseSeriesCodec = serviceProvider.GetRequiredService<BaseSeriesCodec>();
        chineseTelegraphCodec = serviceProvider.GetRequiredService<ChineseTelegraphCodec>();

        // 用于自动关闭 TeachingTip 的计时器
        timer = new DispatcherTimer();
        timer.Tick += Timer_Tick;
        timer.Interval = TimeSpan.FromMilliseconds(800);

        // 获取设置项
        isEncodeWithSpaceChecked = appSettings.IsUtfEncodeWithSpace;
        selectedBaseSeriesTextPreprocessMode = appSettings.BaseSeriesTextPreprocessMode;
        selectedBase58Style = appSettings.Base58Style;
        selectedChineseTelegraphCodeStyle = appSettings.ChineseTelegraphCodeStyle;

        // 初始化部分属性
        CurrentCodecModeText = GetTranslation("CodecPageModeSelectButton/Content");
        CurrentCaesarShift = 0;
        currentCaesarShiftText = string.Empty;
        CaesarCipherNumberFormatter = new()
        {
            FractionDigits = 0,
            NumberRounder = new IncrementNumberRounder()
            {
                Increment = 1,
                RoundingAlgorithm = RoundingAlgorithm.RoundTowardsZero,
            },
        };
    }

    public string GetTranslation(string resource)
    {
        return resourceLoader.GetString(resource);
    }

    private string GetEncodedText(string? rawText)
    {
        if (rawText is null) return string.Empty;

        return CurrentCodecMode switch
        {
            CodecMode.UnicodeBin => UnicodeCodec.BinEncoder(rawText),
            CodecMode.UnicodeOct => UnicodeCodec.OctEncoder(rawText),
            CodecMode.UnicodeDec => UnicodeCodec.DecEncoder(rawText),
            CodecMode.UnicodeHex => UnicodeCodec.HexEncoder(rawText),

            CodecMode.UTF8 => utfCodec.Utf8Encoder(rawText),
            CodecMode.UTF16LE => utfCodec.Utf16LeEncoder(rawText),
            CodecMode.UTF16BE => utfCodec.Utf16BeEncoder(rawText),

            CodecMode.Base64 => baseSeriesCodec.Base64Encoder(rawText),
            CodecMode.Base58 => baseSeriesCodec.Base58Encoder(rawText),
            CodecMode.Base32 => baseSeriesCodec.Base32Encoder(rawText),

            CodecMode.JsonString => JsonStringCodec.Encoder(rawText),
            CodecMode.InternationalMorseCode => MorseCodeCodec.Encoder(rawText),
            CodecMode.ChineseTelegraphCode => chineseTelegraphCodec.Encoder(rawText),

            CodecMode.CaesarCipher => CaesarCipher.Encoder(rawText, CurrentCaesarShift),

            _ => rawText
        };
    }

    private string GetDecodedText(string? encodedText)
    {
        if (encodedText is null) return string.Empty;

        return CurrentCodecMode switch
        {
            CodecMode.UnicodeBin => UnicodeCodec.BinDecoder(encodedText),
            CodecMode.UnicodeOct => UnicodeCodec.OctDecoder(encodedText),
            CodecMode.UnicodeDec => UnicodeCodec.DecDecoder(encodedText),
            CodecMode.UnicodeHex => UnicodeCodec.HexDecoder(encodedText),

            CodecMode.UTF8 => utfCodec.Utf8Decoder(encodedText),
            CodecMode.UTF16LE => utfCodec.Utf16LeDecoder(encodedText),
            CodecMode.UTF16BE => utfCodec.Utf16BeDecoder(encodedText),

            CodecMode.Base64 => baseSeriesCodec.Base64Decoder(encodedText),
            CodecMode.Base58 => baseSeriesCodec.Base58Decoder(encodedText),
            CodecMode.Base32 => baseSeriesCodec.Base32Decoder(encodedText),

            CodecMode.JsonString => JsonStringCodec.Decoder(encodedText),
            CodecMode.InternationalMorseCode => MorseCodeCodec.Decoder(encodedText),
            CodecMode.ChineseTelegraphCode => chineseTelegraphCodec.Decoder(encodedText),

            CodecMode.CaesarCipher => CaesarCipher.Decoder(encodedText, CurrentCaesarShift),

            _ => encodedText
        };
    }

    public void SwitchToEncodeMode()
    {
        LastFocusedIsRawTextBox = true;
    }

    public void SwitchToDecodeMode()
    {
        LastFocusedIsRawTextBox = false;
    }

    /// <summary>
    /// 异步开始编码。
    /// </summary>
    /// <returns>编码的 <see cref="Task"/>（实际为 <see cref="void"/>）</returns>
    public async Task StartEncodeAsync()
    {
        if (LastFocusedIsRawTextBox)
        {
            EncodedText = await Task.Run(() => GetEncodedText(RawText));
        }
    }

    /// <summary>
    /// 异步开始解码。
    /// </summary>
    /// <returns>解码的 <see cref="Task"/>（实际为 <see cref="void"/>）</returns>
    public async Task StartDecodeAsync()
    {
        if (!LastFocusedIsRawTextBox)
        {
            RawText = await Task.Run(() => GetDecodedText(EncodedText));
        }
    }

    /// <summary>
    /// 异步开始编解码。
    /// 什么玩意，就这个不能自动生成 <see cref="IAsyncRelayCommand"/>，只能手写。
    /// </summary>
    /// <returns>编解码的 <see cref="Task"/>（实际为 <see cref="void"/>）</returns>
    public async Task StartCodecAsync()
    {
        if (LastFocusedIsRawTextBox)
        {
            EncodedText = await Task.Run(() => GetEncodedText(RawText));
        }
        else
        {
            RawText = await Task.Run(() => GetDecodedText(EncodedText));
        }
    }

    private AsyncRelayCommand? startCodecAsyncCommand;

    public IAsyncRelayCommand StartCodecAsyncCommand => startCodecAsyncCommand ??= new AsyncRelayCommand(StartCodecAsync);

    /// <summary>
    /// 选择模式时触发。
    /// 设定附加选项的显示情况。
    /// </summary>
    /// <param name="sender">选中的模式对应的控件</param>
    [RelayCommand]
    private async Task SelectCodecMode(object? sender)
    {
        if (sender is null) return;

        CurrentCodecModeText = sender.GetPropertyValue<string>("Text");
        CurrentCodecMode = (CodecMode)Enum.Parse(typeof(CodecMode), sender.GetPropertyValue<string>("Name"));

        if (CurrentCodecMode is >= CodecMode.UTF8 and <= CodecMode.UTF16BE)
            IsEncodeWithSpaceVisible = true;
        else
            IsEncodeWithSpaceVisible = false;

        if (CurrentCodecMode is >= CodecMode.Base64 and <= CodecMode.Base32)
            IsTextPreprocessModeVisible = true;
        else
            IsTextPreprocessModeVisible = false;

        if (CurrentCodecMode == CodecMode.Base58)
            IsBase58StyleVisible = true;
        else
            IsBase58StyleVisible = false;

        if (CurrentCodecMode == CodecMode.ChineseTelegraphCode)
            IsChineseTeleCodeStyleVisible = true;
        else
            IsChineseTeleCodeStyleVisible = false;

        if (CurrentCodecMode == CodecMode.CaesarCipher)
            IsCaesarCipherShiftVisible = true;
        else
            IsCaesarCipherShiftVisible = false;

        await StartCodecAsync();
    }

    public async Task CaeserShiftValueChanged()
    {
        currentCaesarShiftChanged = new();
        await currentCaesarShiftChanged.Task;
        await StartCodecAsync();
    }

    [RelayCommand]
    private void ClearText()
    {
        RawText = string.Empty;
        EncodedText = string.Empty;
    }

    [RelayCommand]
    private void CopyRawText()
    {
        DataPackage package = new();
        package.SetText(RawText);
        Clipboard.SetContent(package);
        IsRawTextCopiedTipOpen = true;
        timer.Stop();
        timer.Start();
    }

    [RelayCommand]
    private async Task PasteRawText()
    {
        SwitchToEncodeMode();
        var package = Clipboard.GetContent();
        if(package.Contains(StandardDataFormats.Text))
        {
            RawText = await package.GetTextAsync();
        }
        IsRawTextPastedTipOpen = true;
        timer.Stop();
        timer.Start();
    }

    [RelayCommand]
    private void CopyEncodedText()
    {
        DataPackage package = new();
        package.SetText(EncodedText);
        Clipboard.SetContent(package);
        IsEncodedTextCopiedTipOpen = true;
        timer.Stop();
        timer.Start();
    }

    [RelayCommand]
    private async Task PasteEncodedText()
    {
        SwitchToDecodeMode();
        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.Text))
        {
            EncodedText = await package.GetTextAsync();
        }
        IsEncodedTextPastedTipOpen = true;
        timer.Stop();
        timer.Start();
    }

    private void Timer_Tick(object? sender, object e)
    {
        if (sender is null) return;
        (sender as DispatcherTimer).Stop();
        IsRawTextCopiedTipOpen = false;
        IsRawTextPastedTipOpen = false;
        IsEncodedTextCopiedTipOpen = false;
        IsEncodedTextPastedTipOpen = false;
    }

    #endregion
}
