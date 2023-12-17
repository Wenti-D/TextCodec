using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TextCodec.Helpers;

namespace TextCodec.Core;

public class UtfCodec
{
    private readonly AppSettings appSettings;
    private readonly IServiceProvider serviceProvider;

    public UtfCodec()
    {
        serviceProvider = Ioc.Default;
        appSettings = serviceProvider.GetRequiredService<AppSettings>();
    }

    public string Utf8Encoder(string raw_text) => Encoder(raw_text, new UTF8Encoding());
    public string Utf8Decoder(string text) => Decoder(text, new UTF8Encoding());
    public string Utf16LeEncoder(string raw_text) => Encoder(raw_text, new UnicodeEncoding());
    public string Utf16LeDecoder(string text) => Decoder(text, new UnicodeEncoding());
    public string Utf16BeEncoder(string raw_text) => Encoder(raw_text, new UnicodeEncoding(true, false));
    public string Utf16BeDecoder(string text) => Decoder(text, new UnicodeEncoding(true, false));

    private string Encoder(string raw_text, Encoding encoding)
    {
        StringBuilder result_buff = new();
        foreach (byte b in encoding.GetBytes(raw_text))
        {
            result_buff.Append(b.ToString("X2"));
            if (appSettings.IsUtfEncodeWithSpace)
                result_buff.Append(' ');
        }
        if (result_buff.Length > 0 && appSettings.IsUtfEncodeWithSpace)
            result_buff.Length--;
        return result_buff.ToString();
    }

    private static string Decoder(string encoded_text, Encoding encoding)
    {
        StringBuilder result_buff = new(), tmp_buff = new();
        List<byte> bytes = new();
        bool is_valid = true;
        foreach (char ch in encoded_text)
        {
            if (Utilities.IsHexChar(ch))
            {
                tmp_buff.Append(ch);
                if (tmp_buff.Length == 2)
                {
                    Utilities.SwitchToValid(ref is_valid, result_buff);
                    bytes.Add(byte.Parse(tmp_buff.ToString(), NumberStyles.AllowHexSpecifier));
                    tmp_buff.Clear();
                }
            }
            else if (char.IsWhiteSpace(ch))
            {
                if (tmp_buff.Length == 1)
                {
                    if (bytes.Count > 0)
                    {
                        is_valid = false;
                        result_buff.Append(encoding.GetString(bytes.ToArray()));
                        bytes.Clear();
                        result_buff.Append(" ⁅");
                    }
                    result_buff.Append(tmp_buff);
                    result_buff.Append(ch);
                    tmp_buff.Clear();
                }
                else if (!is_valid)
                {
                    result_buff.Append(ch);
                }
            }
            else
            {
                result_buff.Append(encoding.GetString(bytes.ToArray()));
                bytes.Clear();
                if (is_valid)
                {
                    is_valid = false;
                    result_buff.Append(" ⁅");
                    result_buff.Append(tmp_buff);
                    tmp_buff.Clear();
                }
                result_buff.Append(ch);
            }
        }
        if (is_valid)
        {
            result_buff.Append(encoding.GetString(bytes.ToArray()));
            bytes.Clear();
            if (tmp_buff.Length > 0)
            {
                result_buff.Append(" ⁅");
            }
        }
        result_buff.Append(tmp_buff);
        if (!is_valid || tmp_buff.Length > 0)
        {
            result_buff.Append('⁆');
        }
        return result_buff.ToString();
    }
}
