using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TextCodec.Core;

namespace TextCodec.Helpers
{
    class BaseSeriesHelper
    {
        private static AppSettings AppSettings = MainWindow.AppSettings;
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
            var preprocessor = GetPreprocessMode(AppSettings.BaseSeriesTextPreprocessMode);
            string code_str = GetBase58Style(AppSettings.Base58Style);
            BigInteger big_int = 0;
            foreach (char ch in encoded_text)
            {
                big_int = big_int * 58 + code_str.IndexOf(ch);
            }
            byte[] bytes = big_int.ToByteArray();
            bytes = bytes.Take(bytes.Length - 1).Reverse().ToArray();
            return preprocessor.GetString(bytes);
        }

        public BaseSeriesHelper()
        {
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
