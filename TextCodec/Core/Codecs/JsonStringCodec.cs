using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TextCodec.Helpers;

namespace TextCodec.Core.Codecs;

class JsonStringCodec
{
    public static string Encoder(string raw_text)
    {
        raw_text = raw_text.Replace("\r\n", "\n").Replace("\r", "\n");
        StringBuilder result_buff = new();
        foreach (char ch in raw_text)
        {
            if (ch >= ' ' && ch <= '~')
            {
                result_buff.Append(
                    ch switch
                    {
                        '"' => @"\""",
                        '\\' => @"\\",
                        _ => ch,
                    }
                );
            }
            else
            {
                result_buff.Append(
                    ch switch
                    {
                        '\t' => @"\t",
                        '\n' => @"\n",
                        '\f' => @"\f",
                        '\b' => @"\b",
                        '\r' => @"\r",
                        _ => string.Format("\\u{0:x4}", (int)ch),
                    }
                );
            }
        }
        return result_buff.ToString();
    }

    public static string Decoder(string encoded_text)
    {
        string[] encoded_texts = encoded_text.Split(
            new char[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries);
        List<StringBuilder> result_buffs = new();
        StringBuilder tmp_buff = new();
        for (int i = 0; i < encoded_texts.Length; i++)
        {
            result_buffs.Add(new StringBuilder());
            tmp_buff.Clear();
            bool is_valid = true;
            bool is_backslash_mode = false, is_uni_mode = false;
            int uni_cnt = 0;
            foreach (char ch in encoded_texts[i])
            {
                if (is_backslash_mode)
                {
                    is_backslash_mode = false;
                    if ("\"btnfr\\".Contains(ch))
                    {
                        tmp_buff.Clear();
                        Utilities.SwitchToValid(ref is_valid, result_buffs[i]);
                        result_buffs[i].Append(
                            ch switch
                            {
                                '"' => '"',
                                'b' => '\b',
                                't' => '\t',
                                'n' => '\n',
                                'f' => '\f',
                                'r' => '\r',
                                '\\' => '\\',
                            }
                        );
                        continue;
                    }
                    else if (ch == 'u')
                    {
                        is_uni_mode = true;
                        continue;
                    }
                    else
                    {
                        Utilities.SwitchToInvalid(ref is_valid, result_buffs[i]);
                        result_buffs[i].Append('\\');
                    }
                }
                if (is_uni_mode)
                {
                    if (Utilities.IsHexChar(ch))
                    {
                        tmp_buff.Append(ch);
                        uni_cnt++;
                        if (uni_cnt == 4)
                        {
                            uni_cnt = 0;
                            is_uni_mode = false;
                            Utilities.SwitchToValid(ref is_valid, result_buffs[i]);
                            result_buffs[i].Append((char)Int16.Parse(tmp_buff.ToString(), NumberStyles.AllowHexSpecifier));
                            tmp_buff.Clear();
                        }
                        continue;
                    }
                    else
                    {
                        uni_cnt = 0;
                        is_uni_mode = false;
                        Utilities.SwitchToInvalid(ref is_valid, result_buffs[i]);
                        result_buffs[i].Append("\\u");
                        result_buffs[i].Append(tmp_buff);
                        tmp_buff.Clear();
                    }
                }
                if (ch == '\\')
                {
                    is_backslash_mode = true;
                }
                else if (ch == '"' || ch < ' ' || ch == 0x7f)
                {
                    Utilities.SwitchToInvalid(ref is_valid, result_buffs[i]);
                    result_buffs[i].Append(ch);
                }
                else
                {
                    Utilities.SwitchToValid(ref is_valid, result_buffs[i]);
                    result_buffs[i].Append(ch);
                }
            }
            if (is_backslash_mode || is_uni_mode)
            {
                Utilities.SwitchToInvalid(ref is_valid, result_buffs[i]);
                result_buffs[i].Append('\\');
                if (is_uni_mode)
                {
                    result_buffs[i].Append('u');
                    result_buffs[i].Append(tmp_buff);
                }
            }
            Utilities.SwitchToValid(ref is_valid, result_buffs[i]);
        }
        return string.Join('\n', result_buffs);
    }
}
