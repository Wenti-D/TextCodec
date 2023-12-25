using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Core;

class CaesarCipher
{
    public static string Encoder(string raw_string, int shift) => CaesarShift(raw_string, shift);
    public static string Decoder(string encoded_string, int shift) => CaesarShift(encoded_string, -shift);

    private static string CaesarShift(string raw_string, int shift)
    {
        StringBuilder result_buff = new();
        foreach (char ch in raw_string)
        {
            if (char.IsAsciiLetterUpper(ch))
            {
                char tmp = (char)(ch + shift % 26);
                result_buff.Append((char)(tmp > 'Z' ? tmp - 26 : tmp < 'A' ? tmp + 26 : tmp));
            }
            else if (char.IsAsciiLetterLower(ch))
            {
                char tmp = (char)(ch + shift % 26);
                result_buff.Append((char)(tmp > 'z' ? tmp - 26 : tmp < 'a' ? tmp + 26 : tmp));
            }
            else
            {
                result_buff.Append(ch);
            }
        }
        return result_buff.ToString();
    }
}
