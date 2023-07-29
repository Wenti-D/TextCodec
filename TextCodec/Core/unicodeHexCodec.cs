using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Core
{
    class UnicodeHexCodec
    {
        public string hexEncoder(string rawText)
        {
            int[] codepoints = strToCodepoints(rawText);
            return codepointsToHexStr(codepoints);
        }

        private int[] strToCodepoints(string text)
        {
            text = text.Replace("\r\n", "\n");
            text = text.Replace("\r", "\n");
            var runes = text.EnumerateRunes();
            int[] codepoints = new int[runes.Count()];
            int i = 0;
            foreach (var rune in runes)
            {
                codepoints[i++] = rune.Value;
            }
            return codepoints;
        }

        private string codepointsToHexStr(int[] codepoints)
        {
            string[] strings = new string[codepoints.Count()];
            int i = 0;
            foreach (int code_value in codepoints)
            {
                strings[i++] = string.Format("{0:X}", code_value);
            }
            return string.Join(" ", strings);
        }

        public string hexDecoder(string text)
        {
            string[] hexStrings = text.Split(
                new char[] { ' ', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);
            string rawText = string.Empty;
            foreach (var hexString in hexStrings)
            {
                if (int.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out int code_value)
                    && Rune.IsValid(code_value))
                {
                    rawText += new Rune(code_value).ToString();
                }
                else
                {
                    rawText += " " + hexString + " ";
                }
            }
            return rawText;
        }
    }
}
