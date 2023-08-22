using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;
using TextCodec.Views.Pages;
using Windows.Storage;

namespace TextCodec.Core
{
    partial class UtfCodec
    {
        public static string Utf8Encoder(string raw_text) => Encoder(raw_text, new UTF8Encoding());
        public static string Utf8Decoder(string text) => Decoder(text, new UTF8Encoding());
        public static string Utf16LeEncoder(string raw_text) => Encoder(raw_text, new UnicodeEncoding());
        public static string Utf16LeDecoder(string text) => Decoder(text, new UnicodeEncoding());
        public static string Utf16BeEncoder(string raw_text) => Encoder(raw_text, new UnicodeEncoding(true, false));
        public static string Utf16BeDecoder(string text) => Decoder(text, new UnicodeEncoding(true, false));

        private static string Encoder(string raw_text, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(raw_text);
            string[] res = new string[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                res[i] = bytes[i].ToString("X2");
            }
            if (ApplicationData.Current.LocalSettings.Values["IsUtfEncodeWithSpaceChecked"] is true)
                return string.Join(" ", res);
            else
                return string.Join("", res);
        }

        private static string Decoder(string text, Encoding encoding)
        {
            var list = new List<object>();
            int index = 0;
            string tmp = string.Empty;
            while (index < text.Length)
            {
                if (tmp.Length == 2)
                {
                    list.Add(byte.Parse(tmp, NumberStyles.AllowHexSpecifier));
                    tmp = string.Empty;
                    if (IsHexChar(text[index]))
                    {
                        tmp += text[index];
                    }
                    else if (!BlankRegex().Match(text[index].ToString()).Success)
                    {
                        AppendString(list, text[index].ToString());
                    }
                }
                else
                {
                    if (IsHexChar(text[index]))
                    {
                        tmp += text[index];
                    }
                    else
                    {
                        AppendString(list, tmp + BlankRegex().Replace(text[index].ToString(), " "));
                        tmp = string.Empty;
                    }
                }
                index++;
            }
            switch (tmp.Length)
            {
                case 2:
                    list.Add(byte.Parse(tmp, NumberStyles.AllowHexSpecifier));
                    break;
                case 1:
                    AppendString(list, tmp);
                    break;
            }
            string res = string.Empty;
            index = 0;
            while (index < list.Count)
            {
                if (list[index] is string)
                {
                    res += " ⁅" + list[index++] + "⁆ ";
                }
                else
                {
                    int index_start = index;
                    while (index < list.Count && list[index] is byte) index++;
                    res += encoding.GetString(Converters.ListToByteArrayConverter.Convert(list.GetRange(index_start, index - index_start)));
                }
            }
            return res;
        }

        private static bool IsHexChar(char ch)
        {
            return ch is
                >= '0' and <= '9' or
                >= 'a' and <= 'f' or
                >= 'A' and <= 'F';
        }

        private static void AppendString(List<object> list, string text)
        {
            if (list.Count > 0 && list[^1] is string)
            {
                list[^1] += text;
            }
            else
            {
                list.Add(text);
            }
        }

        [GeneratedRegex("\\s")]
        private static partial Regex BlankRegex();
    }
}
