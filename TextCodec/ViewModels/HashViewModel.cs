using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TextCodec.Core;
using TextCodec.Helpers;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;

namespace TextCodec.ViewModels;
#nullable enable

public partial class HashViewModel : ObservableObject
{
    #region fields

    private readonly AppSettings appSettings;
    private readonly IServiceProvider serviceProvider;
    private readonly ResourceLoader resourceLoader;
    private readonly HashHelper hashHelper;
    private readonly DispatcherTimer timer;

    #endregion

    #region properties

    [ObservableProperty] // 原始文本
    private string? rawText;
    [ObservableProperty] // 编码后文本
    private string? encodedText;
    [ObservableProperty] // 原始文本的「复制成功」提示是否打开
    private bool isRawTextCopiedTipOpen;
    [ObservableProperty] // 原始文本的「粘贴成功」提示是否打开
    private bool isRawTextPastedTipOpen;
    [ObservableProperty] // 编码后文本的「复制成功」提示是否打开
    private bool isEncodedTextCopiedTipOpen;
    [ObservableProperty]
    private HashMode currentHashMode;
    [ObservableProperty]
    private string currentHashModeText;

    private string selectedHashTextPreprocessMode;

    /// <summary>
    /// 当前文本预处理模式
    /// </summary>
    public string SelectedHashTextPreprocessMode
    {
        get { return selectedHashTextPreprocessMode ??= appSettings.HashTextPreprocessMode; }
        set
        {
            SetProperty(ref selectedHashTextPreprocessMode, value);
            appSettings.HashTextPreprocessMode = value;
        }
    }

    #endregion

    #region methods

    public HashViewModel()
    {
        serviceProvider = Ioc.Default;
        appSettings = serviceProvider.GetRequiredService<AppSettings>();
        resourceLoader = serviceProvider.GetRequiredService<ResourceLoader>();
        hashHelper = serviceProvider.GetRequiredService<HashHelper>();

        selectedHashTextPreprocessMode = appSettings.HashTextPreprocessMode;
        CurrentHashModeText = "MD5";
        CurrentHashMode = (HashMode)Enum.Parse(typeof(HashMode), CurrentHashModeText);
        RawText = "";

        // 用于自动关闭 TeachingTip 的计时器
        timer = new DispatcherTimer();
        timer.Tick += Timer_Tick;
        timer.Interval = TimeSpan.FromMilliseconds(800);
    }

    public string GetTranslation(string resource)
    {
        return resourceLoader.GetString(resource);
    }

    /// <summary>
    /// 异步开始计算散列
    /// </summary>
    /// <returns>散列计算的 <see cref="Task"/>（实际为 <see cref="void"/>）</returns>
    public async Task StartHashAsync()
    {
        HashAlgorithm algorithm = CurrentHashMode switch
        {
            HashMode.MD5 => MD5.Create(),
            HashMode.SHA1 => SHA1.Create(),
            HashMode.SHA256 => SHA256.Create(),
            HashMode.SHA512 => SHA512.Create(),
        };
        MemoryStream stream = new(hashHelper.TextToBytes(RawText));
        byte[] hash = await algorithm.ComputeHashAsync(stream);
        StringBuilder str_builder = new();
        foreach (byte b in hash)
        {
            str_builder.AppendFormat("{0:X2}", b);
        }
        EncodedText = str_builder.ToString();
    }

    /// <summary>
    /// 选择模式时触发。
    /// </summary>
    /// <param name="sender">选中的模式对应的控件</param>
    public async Task SelectHashMode()
    {
        CurrentHashMode = (HashMode)Enum.Parse(typeof(HashMode), CurrentHashModeText);
        await StartHashAsync();
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
        var package = Clipboard.GetContent();
        if (package.Contains(StandardDataFormats.Text))
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

    private void Timer_Tick(object? sender, object e)
    {
        if (sender is null) return;
        (sender as DispatcherTimer).Stop();
        IsRawTextCopiedTipOpen = false;
        IsRawTextPastedTipOpen = false;
        IsEncodedTextCopiedTipOpen = false;
    }

    #endregion
}
