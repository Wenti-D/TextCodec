using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextCodec.Helpers;

namespace TextCodec.Core;

class BaseSeriesCodec
{
    private static AppSettings AppSettings = MainWindow.AppSettings;

    public static string Base64Encoder(string raw_text)
    {
        var preprocessor = new BaseSeriesHelper().GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode);
        return Convert.ToBase64String(preprocessor.GetBytes(raw_text));
    }

    public static string Base64Decoder(string code_text)
    {
        string[] encoded_texts = code_text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        const string code_str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        var preprocessor = new BaseSeriesHelper().GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode);

        List<StringBuilder> result_buffs = new();
        StringBuilder tmp_string_buff = new();
        List<byte> tmp_bytes = new();

        for (int i = 0; i < encoded_texts.Length; i++)
        {
            bool is_valid = true;
            result_buffs.Add(new StringBuilder());
            foreach (char ch in encoded_texts[i])
            {
                if (code_str.Contains(ch))
                {
                    tmp_string_buff.Append(ch);
                    if (tmp_string_buff.Length == 4)
                    {
                        if (tmp_string_buff[2] == '=')
                        {
                            if (is_valid)
                            {
                                is_valid = false;
                                result_buffs[i].Append(preprocessor.GetString(tmp_bytes.ToArray()));
                                result_buffs[i].Append(" ⁅");
                                tmp_bytes.Clear();
                            }
                            result_buffs[i].Append(tmp_string_buff);
                            tmp_string_buff.Clear();
                        }
                        else
                        {
                            if (!is_valid)
                            {
                                is_valid = true;
                                result_buffs[i].Append("⁆ ");
                            }
                            tmp_bytes.AddRange(Convert.FromBase64String(tmp_string_buff.ToString()));
                            tmp_string_buff.Clear();
                        }
                    }
                }
                else if (ch == '=')
                {
                    switch (tmp_string_buff.Length)
                    {
                        case 2:
                            tmp_string_buff.Append('=');
                            break;
                        case 3:
                            if (!is_valid)
                            {
                                is_valid = true;
                                result_buffs[i].Append("⁆ ");
                            }
                            tmp_string_buff.Append('=');
                            tmp_bytes.AddRange(Convert.FromBase64String(tmp_string_buff.ToString()));
                            tmp_string_buff.Clear();
                            break;
                        default:
                            tmp_string_buff.Append(ch);
                            if (is_valid)
                            {
                                is_valid = false;
                                result_buffs[i].Append(preprocessor.GetString(tmp_bytes.ToArray()));
                                result_buffs[i].Append(" ⁅");
                                tmp_bytes.Clear();
                            }
                            result_buffs[i].Append(tmp_string_buff);
                            tmp_string_buff.Clear();
                            break;
                    }
                }
                else
                {
                    if (is_valid)
                    {
                        is_valid = false;
                        result_buffs[i].Append(preprocessor.GetString(tmp_bytes.ToArray()));
                        result_buffs[i].Append(" ⁅");
                        tmp_bytes.Clear();
                        result_buffs[i].Append(tmp_string_buff);
                        tmp_string_buff.Clear();
                    }
                    result_buffs[i].Append(ch);
                }
            }
            result_buffs[i].Append(preprocessor.GetString(tmp_bytes.ToArray()));
            tmp_bytes.Clear();
            if (tmp_string_buff.Length > 0 || !is_valid)
            {
                if (is_valid)
                {
                    result_buffs[i].Append(" ⁅");
                }
                result_buffs[i].Append(tmp_string_buff);
                result_buffs[i].Append("⁆ ");
                tmp_string_buff.Clear();
            }
        }
        return string.Join('\n', result_buffs);
    }

    public static string Base58Encoder(string raw_text)
    {
        BaseSeriesHelper BaseSeriesHelper = new();
        var preprocessor = BaseSeriesHelper.GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode);
        string code_str = BaseSeriesHelper.GetBase58Style(AppSettings.Base58Style);

        List<byte> bytes = preprocessor.GetBytes(raw_text).Reverse().ToList();
        bytes.Add(0);
        BigInteger big_int = new(bytes.ToArray());

        StringBuilder result_buff = new();

        while (big_int > 0)
        {
            result_buff.Insert(0, code_str[(int)(big_int % 58)]);
            big_int /= 58;
        }
        return result_buff.ToString();
    }

    public static string Base58Decoder(string text)
    {
        BaseSeriesHelper BaseSeriesHelper = new();
        string[] encoded_texts = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string code_str = BaseSeriesHelper.GetBase58Style(AppSettings.Base58Style);

        List<StringBuilder> result_buffs = new();
        StringBuilder tmp_buff = new();

        for (int i = 0; i < encoded_texts.Length; i++)
        {
            result_buffs.Add(new StringBuilder());
            bool is_valid = true;
            foreach (char ch in encoded_texts[i])
            {
                if (code_str.Contains(ch))
                {
                    if (!is_valid)
                    {
                        result_buffs[i].Append("⁆ ");
                        is_valid = true;
                    }
                    tmp_buff.Append(ch);
                }
                else
                {
                    result_buffs[i].Append(BaseSeriesHelper.Base58DecodeHelper(tmp_buff.ToString()));
                    tmp_buff.Clear();
                    if (is_valid)
                    {
                        result_buffs[i].Append(" ⁅");
                        is_valid = false;
                    }
                    result_buffs[i].Append(ch);
                }
            }
            if (!is_valid) { result_buffs[i].Append("⁆ "); }
            result_buffs[i].Append(BaseSeriesHelper.Base58DecodeHelper(tmp_buff.ToString()));
        }
        return string.Join("\n", result_buffs);
    }

    public static string Base32Encoder(string raw_text)
    {
        string code_str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        byte[] bytes = new BaseSeriesHelper()
            .GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode)
            .GetBytes(raw_text);
        int group_num = bytes.Length / 5,
            tail_num = bytes.Length % 5;
        StringBuilder result_buff = new();

        for (int i = 0; i < group_num; i++)
        {
            result_buff.Append(code_str[bytes[i] >> 3]);
            result_buff.Append(code_str[((bytes[i] & 0b111) << 2) + (bytes[i + 1] >> 6)]);
            result_buff.Append(code_str[(bytes[i + 1] & 0b111110) >> 1]);
            result_buff.Append(code_str[((bytes[i + 1] & 0b1) << 4) + (bytes[i + 2] >> 4)]);

            result_buff.Append(code_str[((bytes[i + 2] & 0b1111) << 1) + (bytes[i + 3] >> 7)]);
            result_buff.Append(code_str[(bytes[i + 3] & 0b1111100) >> 2]);
            result_buff.Append(code_str[((bytes[i + 3] & 0b11) << 3) + (bytes[i + 4] >> 5)]);
            result_buff.Append(code_str[bytes[i + 4] & 0b11111]);
        }
        int tail_begin = 5 * group_num;
        switch (tail_num)
        {
            case 1:
                result_buff.Append(code_str[bytes[tail_begin] >> 3]);
                result_buff.Append(code_str[(bytes[tail_begin] & 0b111) << 2]);
                result_buff.Append("======");
                break;
            case 2:
                result_buff.Append(code_str[bytes[tail_begin] >> 3]);
                result_buff.Append(code_str[((bytes[tail_begin] & 0b111) << 2) + (bytes[tail_begin + 1] >> 6)]);
                result_buff.Append(code_str[(bytes[tail_begin + 1] & 0b111110) >> 1]);
                result_buff.Append(code_str[(bytes[tail_begin + 1] & 0b1) << 4]);
                result_buff.Append("====");
                break;
            case 3:
                result_buff.Append(code_str[bytes[tail_begin] >> 3].ToString());
                result_buff.Append(code_str[((bytes[tail_begin] & 0b111) << 2) + (bytes[tail_begin + 1] >> 6)]);
                result_buff.Append(code_str[(bytes[tail_begin + 1] & 0b111110) >> 1]);
                result_buff.Append(code_str[((bytes[tail_begin + 1] & 0b1) << 4) + (bytes[tail_begin + 2] >> 4)]);

                result_buff.Append(code_str[(bytes[tail_begin + 2] & 0b1111) << 1]);
                result_buff.Append("===");
                break;
            case 4:
                result_buff.Append(code_str[bytes[tail_begin] >> 3]);
                result_buff.Append(code_str[((bytes[tail_begin] & 0b111) << 2) + (bytes[tail_begin + 1] >> 6)]);
                result_buff.Append(code_str[(bytes[tail_begin + 1] & 0b111110) >> 1]);
                result_buff.Append(code_str[((bytes[tail_begin + 1] & 0b1) << 4) + (bytes[tail_begin + 2] >> 4)]);

                result_buff.Append(code_str[((bytes[tail_begin + 2] & 0b1111) << 1) + (bytes[tail_begin + 3] >> 7)]);
                result_buff.Append(code_str[(bytes[tail_begin + 3] & 0b1111100) >> 2]);
                result_buff.Append(code_str[(bytes[tail_begin + 3] & 0b11) << 3]);
                result_buff.Append('=');
                break;
        }
        return result_buff.ToString();
    }

    public static string Base32Decoder(string encoded_text)
    {
        string[] encoded_texts = encoded_text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string code_str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var preprocessor = new BaseSeriesHelper().GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode);

        List<StringBuilder> result_buffs = new();
        StringBuilder tmp_string_buff = new();
        List<byte> tmp_bytes = new();

        for (int i = 0; i < encoded_texts.Length; i++)
        {
            bool is_valid = true;
            result_buffs.Add(new StringBuilder());
            foreach (char ch in encoded_texts[i])
            {
                if (code_str.Contains(ch))
                {
                    tmp_string_buff.Append(ch);
                    if (tmp_string_buff.Length == 8)
                    {
                        string tmp_string = tmp_string_buff.ToString();
                        if (tmp_string.Contains('='))
                        {
                            if (is_valid)
                            {
                                is_valid = false;
                                result_buffs[i].Append(preprocessor.GetString(tmp_bytes.ToArray()));
                                result_buffs[i].Append(" ⁅");
                                tmp_bytes.Clear();
                            }
                            result_buffs[i].Append(tmp_string_buff);
                            tmp_string_buff.Clear();
                        }
                        else
                        {
                            if (!is_valid)
                            {
                                is_valid = true;
                                result_buffs[i].Append("⁆ ");
                            }
                            tmp_bytes.AddRange(BaseSeriesHelper.Base32DecodeHelper(tmp_string));
                            tmp_string_buff.Clear();
                        }
                    }
                }
                else if (ch == '=')
                {
                    int tmp_string_len = tmp_string_buff.Length;
                    if (tmp_string_len == 2 || tmp_string_len == 4 || tmp_string_len == 5
                        || (tmp_string_len == 3 || tmp_string_len == 6) && tmp_string_buff[tmp_string_len - 1] == '=')
                    {
                        tmp_string_buff.Append(ch);
                    }
                    else if (tmp_string_len == 7)
                    {
                        if (!is_valid)
                        {
                            is_valid = true;
                            result_buffs[i].Append("⁆ ");
                        }
                        tmp_string_buff.Append(ch);
                        tmp_bytes.AddRange(BaseSeriesHelper.Base32DecodeHelper(tmp_string_buff.ToString()));
                        tmp_string_buff.Clear();
                    }
                    else
                    {
                        tmp_string_buff.Append(ch);
                        if (is_valid)
                        {
                            is_valid = false;
                            result_buffs[i].Append(preprocessor.GetString(tmp_bytes.ToArray()));
                            result_buffs[i].Append(" ⁅");
                            tmp_bytes.Clear();
                        }
                        result_buffs[i].Append(tmp_string_buff);
                        tmp_string_buff.Clear();
                    }
                }
                else
                {
                    if (is_valid)
                    {
                        is_valid = false;
                        result_buffs[i].Append(preprocessor.GetString(tmp_bytes.ToArray()));
                        result_buffs[i].Append(" ⁅");
                        tmp_bytes.Clear();
                        result_buffs[i].Append(tmp_string_buff);
                        tmp_string_buff.Clear();
                    }
                    result_buffs[i].Append(ch);
                }
            }
            result_buffs[i].Append(preprocessor.GetString(tmp_bytes.ToArray()));
            tmp_bytes.Clear();
            if (tmp_string_buff.Length > 0 || !is_valid)
            {
                if (is_valid)
                {
                    result_buffs[i].Append(" ⁅");
                }
                result_buffs[i].Append(tmp_string_buff);
                result_buffs[i].Append("⁆ ");
                tmp_string_buff.Clear();
            }
        }
        return string.Join("\n", result_buffs);
    }
}
