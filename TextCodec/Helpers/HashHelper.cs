using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TextCodec.Core;

namespace TextCodec.Helpers;

public class HashHelper
{
    private readonly AppSettings appSettings;
    private readonly IServiceProvider serviceProvider;
    private readonly Dictionary<string, Encoding> PreprocessMode = new();

    public byte[] TextToBytes(string text)
    {
        Encoding preprocess_mode = PreprocessMode[appSettings.HashTextPreprocessMode];
        return preprocess_mode.GetBytes(text);
    }

    public HashHelper()
    {
        serviceProvider = Ioc.Default;
        appSettings = serviceProvider.GetRequiredService<AppSettings>();

        PreprocessMode.Add("HashPreprocessModeUtf8", new UTF8Encoding());
        PreprocessMode.Add("HashPreprocessModeUtf16Le", new UnicodeEncoding());
        PreprocessMode.Add("HashPreprocessModeUtf16Be", new UnicodeEncoding(true, false));
    }
}
