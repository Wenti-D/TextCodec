using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCodec.Helpers;

namespace TextCodec.Core
{
    class JsonStringCodec
    {
        public static string Encoder(string raw_text)
        {
            raw_text = raw_text.Replace("\r\n", "\n").Replace("\r", "\n");
            string result = string.Empty;
            foreach (char ch in raw_text)
            {
                if (ch >= ' ' && ch <= '~')
                {
                    result += ch switch
                    {
                        '"' => @"\""",
                        '\\' => @"\\",
                        _ => ch,
                    };
                }
                else
                {
                    result += ch switch
                    {
                        '\t' => @"\t",
                        '\n' => @"\n",
                        '\f' => @"\f",
                        '\b' => @"\b",
                        '\r' => @"\r",
                        _ => string.Format("\\u{0:x4}", (int)ch),
                    };
                }
            }
            return result;
        }

        public static string Decoder(string encoded_text)
        {
            string[] encoded_texts = encoded_text.Split(
                new char[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);
            string[] results = new string[encoded_texts.Length];
            for (int i = 0; i < encoded_texts.Length; i++)
            {
                string tmp_str = string.Empty;
                List<char> tmp_chars = new();
                bool valid_ch = true;
                int j = 0;
                while (j < encoded_texts[i].Length)
                {
                    if (encoded_texts[i][j] == '\\')
                    {
                        char char_after_backslash;
                        try
                        {
                            char_after_backslash = encoded_texts[i][++j];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            if (valid_ch)
                            {
                                valid_ch = false;
                                results[i] += new string(tmp_chars.ToArray()) + " ⁅";
                                tmp_chars.Clear();
                            }
                            results[i] += "\\";
                            break;
                        }
                        if ("\"btnfr\\".Contains(char_after_backslash))
                        {
                            if (!valid_ch)
                            {
                                valid_ch = true;
                                results[i] += "⁆ ";
                            }
                            results[i] += new string(tmp_chars.ToArray()) + char_after_backslash switch
                            {
                                '"' => '"',
                                'b' => '\b',
                                't' => '\t',
                                'n' => '\n',
                                'f' => '\f',
                                'r' => '\r',
                                '\\' => '\\',
                            };
                            j++;
                        }
                        else if (char_after_backslash == 'u')
                        {
                            j++;
                            for (int _ = 0; _ < 4; _++)
                            {
                                try
                                {
                                    if (Utilities.IsHexChar(encoded_texts[i][j]))
                                    {
                                        tmp_str += encoded_texts[i][j];
                                        j++;
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }
                                }
                                catch (Exception)
                                {
                                    if (valid_ch)
                                    {
                                        valid_ch = false;
                                        results[i] += new string(tmp_chars.ToArray()) + " ⁅";
                                        tmp_chars.Clear();
                                    }
                                    results[i] += "\\u" + tmp_str;
                                    tmp_str = string.Empty;
                                    break;
                                }
                            }
                            if (tmp_str.Length == 4)
                            {
                                if (!valid_ch)
                                {
                                    valid_ch = true;
                                    results[i] += "⁆ ";
                                }
                                tmp_chars.Add((char)Int16.Parse(tmp_str, NumberStyles.AllowHexSpecifier));
                                tmp_str = string.Empty;
                            }
                        }
                        else
                        {
                            if (valid_ch)
                            {
                                valid_ch = false;
                                results[i] += new string(tmp_chars.ToArray()) + " ⁅";
                                tmp_chars.Clear();
                            }
                            results[i] += "\\";
                        }
                    }
                    else if (encoded_texts[i][j] == '"' || encoded_texts[i][j] < ' ' || encoded_texts[i][j] == (char)0x7f)
                    {
                        results[i] += new string(tmp_chars.ToArray());
                        tmp_chars.Clear();
                        if (valid_ch)
                        {
                            valid_ch = false;
                            results[i] += " ⁅";
                        }
                        results[i] += encoded_texts[i][j];
                        j++;
                    }
                    else
                    {
                        if (!valid_ch)
                        {
                            valid_ch = true;
                            results[i] += "⁆ ";
                        }
                        results[i] += new string(tmp_chars.ToArray());
                        tmp_chars.Clear();
                        results[i] += encoded_texts[i][j];
                        j++;
                    }
                }
                if (!valid_ch)
                {
                    results[i] += tmp_str + "⁆ ";
                }
                else
                {
                    results[i] += new string(tmp_chars.ToArray());
                }
            }
            return string.Join("\n", results);
        }
    }
}
