using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextCodec.Helpers;

namespace TextCodec.Core
{
    partial class BaseSeriesCodec
    {
        [GeneratedRegex("[A-Za-z0-9+/]")]
        private static partial Regex Base64Char();
        [GeneratedRegex("[A-Za-z0-9+/=]")]
        private static partial Regex Base64CharEnd();

        private static AppSettings AppSettings = MainWindow.AppSettings;

        public static string Base64Encoder(string raw_text)
        {
            var preprocessor = new BaseSeriesHelper().GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode);
            return Convert.ToBase64String(preprocessor.GetBytes(raw_text));
        }

        public static string Base64Decoder(string text)
        {
            string[] encoded_texts = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] results = new string[encoded_texts.Length];
            var preprocessor = new BaseSeriesHelper().GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode);
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
                            results[i] += preprocessor.GetString(Convert.FromBase64String(tmp + ch));
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

        public static string Base58Encoder(string raw_text)
        {
            BaseSeriesHelper BaseSeriesHelper = new();
            var preprocessor = BaseSeriesHelper.GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode);
            string code_str = BaseSeriesHelper.GetBase58Style(AppSettings.Base58Style);
            List<byte> bytes = preprocessor.GetBytes(raw_text).Reverse().ToList();
            bytes.Add(0);
            BigInteger big_int = new(bytes.ToArray());
            List<char> chars = new();
            while (big_int > 0)
            {
                chars.Insert(0, code_str[(int)(big_int % 58)]);
                big_int /= 58;
            }
            return string.Join("", chars.ToArray());
        }

        public static string Base58Decoder(string text)
        {
            BaseSeriesHelper BaseSeriesHelper = new();
            string[] encoded_texts = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] results = new string[encoded_texts.Length];
            string code_str = BaseSeriesHelper.GetBase58Style(AppSettings.Base58Style);
            for (int i = 0; i < encoded_texts.Length; i++)
            {
                string tmp = string.Empty;
                bool valid_ch = true;
                foreach (char ch in encoded_texts[i])
                {
                    if (!code_str.Contains(ch))
                    {
                        results[i] += BaseSeriesHelper.Base58DecodeHelper(tmp);
                        tmp = string.Empty;
                        if (valid_ch)
                        {
                            results[i] += " ⁅";
                            valid_ch = false;
                        }
                        results[i] += ch;
                    }
                    else
                    {
                        if (!valid_ch)
                        {
                            results[i] += "⁆ ";
                            valid_ch = true;
                        }
                        tmp += ch;
                    }
                }
                if (!valid_ch) { results[i] += "⁆ "; }
                results[i] += BaseSeriesHelper.Base58DecodeHelper(tmp);
            }
            return string.Join("\n", results);
        }
    }
}
