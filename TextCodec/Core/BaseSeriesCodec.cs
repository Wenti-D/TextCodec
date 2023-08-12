using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextCodec.Core
{
    class BaseSeriesCodec
    {
        public static string Base64Encoder(string raw_text)
        {
            var utf8 = new UTF8Encoding();
            return Convert.ToBase64String(utf8.GetBytes(raw_text));
        }

        public static string Base64Decoder(string text)
        {
            string[] encoded_texts = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] results = new string[encoded_texts.Length];
            var utf8 = new UTF8Encoding();
            for (int i = 0; i < encoded_texts.Length; i++)
            {
                string tmp = string.Empty;
                bool valid_ch = true;
                for (int j = 0; j < encoded_texts[i].Length; j++)
                {
                    char ch = encoded_texts[i][j];
                    switch (tmp.Length)
                    {
                        case 4:
                            if (!valid_ch)
                            {
                                results[i] += "⁆ ";
                                valid_ch = true;
                            }
                            results[i] += utf8.GetString(Convert.FromBase64String(tmp));
                            if (Regex.Match(ch.ToString(), "[A-Za-z0-9+/]").Success)
                            {
                                tmp = ch.ToString();
                            }
                            else
                            {
                                valid_ch = false;
                                tmp = string.Empty;
                                results[i] += " ⁅" + ch;
                            }
                            break;
                        case 3:
                            if (Regex.Match(ch.ToString(), "[A-Za-z0-9+/=]").Success)
                            {
                                tmp += ch;
                            }
                            else
                            {
                                if (valid_ch)
                                {
                                    results[i] += " ⁅";
                                    valid_ch = false;
                                }
                                results[i] += tmp + ch;
                                tmp = string.Empty;
                            }
                            break;
                        case 2:
                            if (Regex.Match(ch.ToString(), "[A-Za-z0-9+/=]").Success)
                            {
                                tmp += ch;
                            }
                            else
                            {
                                if (valid_ch)
                                {
                                    results[i] += " ⁅";
                                    valid_ch = false;
                                }
                                results[i] += tmp + ch;
                                tmp = string.Empty;
                            }
                            break;
                        default:
                            if (Regex.Match(ch.ToString(), "[A-Za-z0-9+/]").Success)
                            {
                                tmp += ch;
                            }
                            else
                            {
                                if (valid_ch)
                                {
                                    results[i] += " ⁅";
                                    valid_ch = false;
                                }
                                tmp = string.Empty;
                                results[i] += ch;
                            }
                            break;
                    }
                }
                if (tmp.Length == 4)
                {
                    if (!valid_ch)
                    {
                        results[i] += "⁆ ";
                    }
                    results[i] += utf8.GetString(Convert.FromBase64String(tmp));
                }
                else
                {
                    if (valid_ch)
                    {
                        results[i] += " ⁅";
                    }
                    results[i] += tmp + "⁆ ";
                }
            }
            return string.Join("\n", results);
        }
    }
}
