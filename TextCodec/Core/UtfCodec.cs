using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;
using Windows.Storage;
using TextCodec.Helpers;

namespace TextCodec.Core
{
    partial class UtfCodec
    {
        private static AppSettings AppSettings = MainWindow.AppSettings;

        public static string Utf8Encoder(string raw_text) => Encoder(raw_text, new UTF8Encoding());
        public static string Utf8Decoder(string text) => Decoder(text, new UTF8Encoding());
        public static string Utf16LeEncoder(string raw_text) => Encoder(raw_text, new UnicodeEncoding());
        public static string Utf16LeDecoder(string text) => Decoder(text, new UnicodeEncoding());
        public static string Utf16BeEncoder(string raw_text) => Encoder(raw_text, new UnicodeEncoding(true, false));
        public static string Utf16BeDecoder(string text) => Decoder(text, new UnicodeEncoding(true, false));

        private static string Encoder(string raw_text, Encoding encoding)
        {
            StringBuilder result_buff = new();
            foreach (byte b in encoding.GetBytes(raw_text))
            {
                result_buff.Append(b.ToString("X2"));
                if (AppSettings.IsUtfEncodeWithSpace)
                    result_buff.Append(' ');
            }
            if (result_buff.Length > 0 && AppSettings.IsUtfEncodeWithSpace)
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
                        if (!is_valid)
                        {
                            result_buff.Append("⁆ ");
                            is_valid = true;
                        }
                        bytes.Add(byte.Parse(tmp_buff.ToString(), NumberStyles.AllowHexSpecifier));
                        tmp_buff.Clear();
                    }
                }
                else if (BlankRegex().Match(ch.ToString()).Success)
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
                result_buff.Append("⁆ ");
            }
            return result_buff.ToString();
        }

        [GeneratedRegex("\\s")]
        private static partial Regex BlankRegex();
    }
}
