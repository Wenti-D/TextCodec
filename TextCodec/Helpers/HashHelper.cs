using System.Collections.Generic;
using System.Text;
using TextCodec.Core;

namespace TextCodec.Helpers
{
    class HashHelper
    {
        private static AppSettings AppSettings = MainWindow.AppSettings;
        private Dictionary<string, Encoding> PreprocessMode = new();

        public byte[] TextToBytes(string text)
        {
            Encoding preprocess_mode = PreprocessMode[AppSettings.HashTextPreprocessMode];
            return preprocess_mode.GetBytes(text);
        }

        public HashHelper()
        {
            PreprocessMode.Add("HashPreprocessModeUtf8", new UTF8Encoding());
            PreprocessMode.Add("HashPreprocessModeUtf16Le", new UnicodeEncoding());
            PreprocessMode.Add("HashPreprocessModeUtf16Be", new UnicodeEncoding(true, false));
        }
    }
}
