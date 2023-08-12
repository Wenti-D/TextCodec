using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextCodec.Core
{
    partial class BaseSeriesCodec
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
                foreach (char ch in encoded_texts[i])
                {
                    if (tmp.Length == 3)
                    {
                        if (!valid_ch) { results[i] += "⁆ "; }
                        if (Base64CharEnd().Match(ch.ToString()).Success)
                        {
                            results[i] += utf8.GetString(Convert.FromBase64String(tmp + ch));
                            valid_ch = true;
                        }
                        else
                        {
                            results[i] += " ⁅" + ch;
                        }
                        tmp = string.Empty;
                    }
                    else if (tmp.Length == 2 && Base64CharEnd().Match(ch.ToString()).Success
                           || tmp.Length < 2 && Base64Char().Match(ch.ToString()).Success)
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
                }
                if (tmp != string.Empty && valid_ch) { results[i] += " ⁅"; }
                if (!(valid_ch && tmp == string.Empty)) { results[i] += tmp + "⁆ "; }
            }
            return string.Join("\n", results);
        }

        [GeneratedRegex("[A-Za-z0-9+/]")]
        private static partial Regex Base64Char();
        [GeneratedRegex("[A-Za-z0-9+/=]")]
        private static partial Regex Base64CharEnd();
    }
}
