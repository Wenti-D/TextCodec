using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Text;
using TextCodec.Helpers;

namespace TextCodec.Core;

public class UnicodeCodec
{
    public static string BinEncoder(string raw_text) => Encoder(raw_text, 2);
    public static string BinDecoder(string encoded_text) => Decoder(encoded_text, 2);
    public static string OctEncoder(string raw_text) => Encoder(raw_text, 8);
    public static string OctDecoder(string encoded_text) => Decoder(encoded_text, 8);
    public static string DecEncoder(string raw_text) => Encoder(raw_text, 10);
    public static string DecDecoder(string encoded_text) => Decoder(encoded_text, 10);
    public static string HexEncoder(string raw_text) => Encoder(raw_text, 16);
    public static string HexDecoder(string encoded_text) => Decoder(encoded_text, 16);


    private static string Encoder(string raw_text, int target_base)
    {
        int[] codepoints = UnicodeConvertHelper.RawStrToCodepoints(raw_text);
        return UnicodeConvertHelper.CodepointsToValueStr(codepoints, target_base);
    }

    private static string Decoder(string encoded_text, int target_base)
    {
        StringBuilder result_buff = new(), tmp_buff = new();
        bool is_valid = true;
        foreach (char ch in encoded_text)
        {
            if (Utilities.IsTargetBaseChar(ch, target_base))
            {
                tmp_buff.Append(ch);
            }
            else
            {
                UnicodeConvertHelper.TryParseCodepointStr(result_buff, tmp_buff, target_base, ref is_valid);
                if (char.IsWhiteSpace(ch))
                {
                    if (!is_valid)
                        result_buff.Append(ch);
                }
                else
                {
                    Utilities.SwitchToInvalid(ref is_valid, result_buff);
                    result_buff.Append(ch);
                }
            }
        }
        UnicodeConvertHelper.TryParseCodepointStr(result_buff, tmp_buff, target_base, ref is_valid);
        if (!is_valid)
        {
            result_buff.Append("⁆ ");
        }
        return result_buff.ToString();
    }
}
