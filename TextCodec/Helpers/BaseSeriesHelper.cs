using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using TextCodec.Core;

namespace TextCodec.Helpers
{
    class BaseSeriesHelper
    {
        private readonly AppSettings appSettings;
        private IServiceProvider serviceProvider;
        private Dictionary<string, Encoding> PreprocessMode = new();
        private Dictionary<string, string> Base58Style = new();

        public Encoding GetPreprocessMode(string selected_mode)
        {
            return PreprocessMode[selected_mode];
        }

        public string GetBase58Style(string selected_style)
        {
            return Base58Style[selected_style];
        }

        public string Base58DecodeHelper(string encoded_text)
        {
            var preprocessor = GetPreprocessMode(appSettings.BaseSeriesTextPreprocessMode);
            string code_str = GetBase58Style(appSettings.Base58Style);
            BigInteger big_int = 0;
            foreach (char ch in encoded_text)
            {
                big_int = big_int * 58 + code_str.IndexOf(ch);
            }
            byte[] bytes = big_int.ToByteArray();
            bytes = bytes.Take(bytes.Length - 1).Reverse().ToArray();
            return preprocessor.GetString(bytes);
        }


        public static List<byte> Base32DecodeHelper(string encoded_text)
        {
            string code_str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            List<byte> bytes = new();
            uint eq_pos = (uint)encoded_text.IndexOf('=');
            if (eq_pos >= 2)
            {
                bytes.Add((byte)
                    ((code_str.IndexOf(encoded_text[0]) << 3)
                    + (code_str.IndexOf(encoded_text[1]) >> 2))
                    );
            }
            if (eq_pos >= 4)
            {
                bytes.Add((byte)
                    (((code_str.IndexOf(encoded_text[1]) & 0b11) << 6)
                    + (code_str.IndexOf(encoded_text[2]) << 1)
                    + (code_str.IndexOf(encoded_text[3]) >> 4))
                    );
            }
            if (eq_pos >= 5)
            {
                bytes.Add((byte)
                    (((code_str.IndexOf(encoded_text[3]) & 0b1111) << 4)
                    + (code_str.IndexOf(encoded_text[4]) >> 1))
                    );
            }
            if (eq_pos >= 7)
            {
                bytes.Add((byte)
                    (((code_str.IndexOf(encoded_text[4]) & 0b1) << 7)
                    + (code_str.IndexOf(encoded_text[5]) << 2)
                    + (code_str.IndexOf(encoded_text[6]) >> 3))
                    );
            }
            if (eq_pos >= 8)
            {
                bytes.Add((byte)
                    (((code_str.IndexOf(encoded_text[6]) & 0b111) << 5)
                    + (code_str.IndexOf(encoded_text[7])))
                    );
            }
            return bytes;
        }

        public BaseSeriesHelper()
        {
            serviceProvider = Ioc.Default;
            appSettings = serviceProvider.GetRequiredService<AppSettings>();

            PreprocessMode.Add("CodecPageModeUtf8", new UTF8Encoding());
            PreprocessMode.Add("CodecPageModeUtf16Le/Text", new UnicodeEncoding());
            PreprocessMode.Add("CodecPageModeUtf16Be/Text", new UnicodeEncoding(true, false));

            Base58Style.Add("CodecPageBase58StdCharList",
                "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ");
            Base58Style.Add("CodecPageBase58BtcCharList",
                "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz");
        }
    }
}
