using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TextCodec.Helpers;

namespace TextCodec.Core;

public class ChineseTelegraphCodec
{
    private readonly AppSettings appSettings;
    private IServiceProvider serviceProvider;

    public ChineseTelegraphCodec()
    {
        serviceProvider = Ioc.Default;
        appSettings = serviceProvider.GetRequiredService<AppSettings>();
    }

    public string Encoder(string raw_text)
    {
        StringBuilder result_buff = new();
        bool is_valid = true;
        foreach (char ch in raw_text.ToUpper())
        {
            try
            {
                string code = Dicts.ChineseTelegraphDict.CharCodenumPairs[ch];
                Utilities.SwitchToValid(ref is_valid, result_buff);
                switch (appSettings.ChineseTelegraphCodeStyle)
                {
                    case "ChineseTeleCodeNumber":
                        result_buff.Append(code);
                        result_buff.Append(' ');
                        break;
                    case "ChineseTeleCodeNumberSimp":
                        foreach (char code_ch in code)
                        {
                            result_buff.Append(Dicts.ChineseTelegraphDict.NumCodeSimpPairs[code_ch]);
                            result_buff.Append(' ');
                        }
                        break;
                    case "ChineseTeleCodeNumberFull":
                        foreach (char code_ch in code)
                        {
                            result_buff.Append(Dicts.ChineseTelegraphDict.NumCodeFullPairs[code_ch]);
                            result_buff.Append(' ');
                        }
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                Utilities.SwitchToInvalid(ref is_valid, result_buff);
                result_buff.Append(ch);
            }
        }
        if (!is_valid)
            result_buff.Append('⁆');
        if (is_valid && result_buff.Length > 0)
            result_buff.Length--;
        return result_buff.ToString();
    }

    public string Decoder(string encoded_text)
    {
        return appSettings.ChineseTelegraphCodeStyle switch
        {
            "ChineseTeleCodeNumber" => NumDecoder(encoded_text),
            "ChineseTeleCodeNumberSimp" => NumMorseDecoder(encoded_text, Dicts.ChineseTelegraphDict.CodeSimpNumPairs),
            "ChineseTeleCodeNumberFull" => NumMorseDecoder(encoded_text, Dicts.ChineseTelegraphDict.CodeFullNumPairs),
            _ => encoded_text,
        };
    }

    private static string NumDecoder(string encoded_text)
    {
        StringBuilder result_buff = new();
        StringBuilder tmp_buff = new();
        bool is_valid = true;
        foreach (char ch in encoded_text)
        {
            if (ch >= '0' && ch <= '9')
            {
                tmp_buff.Append(ch);
                if (tmp_buff.Length == 4)
                {
                    try
                    {
                        char tmp_res = Dicts.ChineseTelegraphDict.CodenumCharPairs[tmp_buff.ToString()];
                        Utilities.SwitchToValid(ref is_valid, result_buff);
                        result_buff.Append(tmp_res);
                    }
                    catch (KeyNotFoundException)
                    {
                        Utilities.SwitchToInvalid(ref is_valid, result_buff);
                        result_buff.Append(tmp_buff);
                    }
                    finally { tmp_buff.Clear(); }
                }
            }
            else if (char.IsWhiteSpace(ch))
            {
                if (tmp_buff.Length > 0)
                {
                    Utilities.SwitchToInvalid(ref is_valid, result_buff);
                    result_buff.Append(tmp_buff);
                    tmp_buff.Clear();
                }
                if (!is_valid)
                {
                    result_buff.Append(ch);
                }
            }
            else
            {
                Utilities.SwitchToInvalid(ref is_valid, result_buff);
                result_buff.Append(tmp_buff);
                result_buff.Append(ch);
                tmp_buff.Clear();
            }
        }
        if (tmp_buff.Length > 0)
        {
            Utilities.SwitchToInvalid(ref is_valid, result_buff);
            result_buff.Append(tmp_buff);
            tmp_buff.Clear();
        }
        if (!is_valid)
        {
            result_buff.Append('⁆');
        }
        return result_buff.ToString();
    }

    private static string NumMorseDecoder(string endcoded_text, IReadOnlyDictionary<string, char> dict)
    {
        bool is_valid = true;
        StringBuilder result_buff = new();
        StringBuilder partial_pending_chars = new();
        StringBuilder partial_decoded_chars = new();
        StringBuilder tmp_buff = new();
        foreach (char ch in endcoded_text)
        {
            if (ch != '.' && ch != '-')
            {
                if (tmp_buff.Length > 0)
                {
                    try
                    {
                        partial_decoded_chars.Append(dict[tmp_buff.ToString()]);
                    }
                    catch (KeyNotFoundException)
                    {
                        Utilities.SwitchToInvalid(ref is_valid, result_buff);
                        result_buff.Append(partial_pending_chars);
                        partial_pending_chars.Clear();
                    }
                    finally
                    {
                        tmp_buff.Clear();
                    }
                }
                if (partial_decoded_chars.Length == 4)
                {
                    try
                    {
                        char tmp = Dicts.ChineseTelegraphDict.CodenumCharPairs[partial_decoded_chars.ToString()];

                        Utilities.SwitchToValid(ref is_valid, result_buff);
                        result_buff.Append(tmp);
                    }
                    catch (KeyNotFoundException)
                    {
                        Utilities.SwitchToInvalid(ref is_valid, result_buff);
                        result_buff.Append(partial_pending_chars);
                    }
                    finally
                    {
                        partial_decoded_chars.Clear();
                        partial_pending_chars.Clear();
                    }
                }
                if (char.IsWhiteSpace(ch))
                {
                    if (partial_pending_chars.Length > 0)
                    {
                        partial_pending_chars.Append(ch);
                    }
                    else if (!is_valid)
                    {
                        result_buff.Append(ch);
                    }
                }
                else
                {
                    Utilities.SwitchToInvalid(ref is_valid, result_buff);
                    result_buff.Append(ch);
                }
            }
            else
            {
                tmp_buff.Append(ch);
                partial_pending_chars.Append(ch);
            }
        }

        if (tmp_buff.Length > 0)
        {
            try
            {
                partial_decoded_chars.Append(dict[tmp_buff.ToString()]);
            }
            catch (KeyNotFoundException)
            {
                Utilities.SwitchToInvalid(ref is_valid, result_buff);
                result_buff.Append(partial_pending_chars);
                partial_pending_chars.Clear();
            }
            finally
            {
                tmp_buff.Clear();
            }
        }
        if (partial_decoded_chars.Length == 4)
        {
            try
            {
                char tmp = Dicts.ChineseTelegraphDict.CodenumCharPairs[partial_decoded_chars.ToString()];

                Utilities.SwitchToValid(ref is_valid, result_buff);
                result_buff.Append(tmp);
            }
            catch (KeyNotFoundException)
            {
                Utilities.SwitchToInvalid(ref is_valid, result_buff);
                result_buff.Append(partial_pending_chars);
            }
            finally
            {
                partial_decoded_chars.Clear();
                partial_pending_chars.Clear();
            }
        }
        else if (partial_pending_chars.Length > 0)
        {
            Utilities.SwitchToInvalid(ref is_valid, result_buff);
            result_buff.Append(partial_pending_chars);
        }
        if (!is_valid) result_buff.Append('⁆');
        return result_buff.ToString();
    }
}
