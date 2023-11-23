using System.Collections.Generic;
using System.Text;
using TextCodec.Core.Dicts;
using TextCodec.Helpers;

namespace TextCodec.Core;

class MorseCodeCodec
{
    public static string Encoder(string raw_text)
    {
        bool is_valid = true;
        var morse_dict = MorseCodeDict.CharMorsePairs;
        StringBuilder encoded_text_buff = new();
        foreach (char ch in raw_text.ToUpper().Replace("%", "-0/0").Replace("‰", "-0/00"))
        {
            try
            {
                string tmp_str = morse_dict[ch];
                Utilities.SwitchToValid(ref is_valid, encoded_text_buff);
                encoded_text_buff.Append(tmp_str);
                encoded_text_buff.Append(' ');
            }
            catch (KeyNotFoundException)
            {
                Utilities.SwitchToInvalid(ref is_valid, encoded_text_buff);
                encoded_text_buff.Append(ch);
            }
        }
        if (!is_valid) encoded_text_buff.Append('⁆');
        return encoded_text_buff.ToString();
    }

    public static string Decoder(string code_text)
    {
        bool is_valid = true;
        var morse_dict = MorseCodeDict.MorseCharPairs;
        StringBuilder decoded_text_buff = new();
        StringBuilder tmp_text_buff = new();
        foreach (char ch in code_text)
        {
            if (ch != ' ' && ch != '.' && ch != '-')
            {
                try
                {
                    char? tmp_ch = morse_dict[tmp_text_buff.ToString()];
                    Utilities.SwitchToValid(ref is_valid, decoded_text_buff);
                    decoded_text_buff.Append(tmp_ch);
                }
                catch (KeyNotFoundException)
                {
                    Utilities.SwitchToInvalid(ref is_valid, decoded_text_buff);
                    decoded_text_buff.Append(tmp_text_buff);
                }
                Utilities.SwitchToInvalid(ref is_valid, decoded_text_buff);
                tmp_text_buff.Clear();
                decoded_text_buff.Append(ch);
            }
            else if (ch == ' ')
            {
                if (tmp_text_buff.Length == 0)
                {
                    tmp_text_buff.Append(ch);
                }
                else if (tmp_text_buff[0] != ' ')
                {
                    try
                    {
                        char? tmp_ch = morse_dict[tmp_text_buff.ToString()];
                        Utilities.SwitchToValid(ref is_valid, decoded_text_buff);
                        decoded_text_buff.Append(tmp_ch);
                    }
                    catch (KeyNotFoundException)
                    {
                        Utilities.SwitchToInvalid(ref is_valid, decoded_text_buff);
                        decoded_text_buff.Append(tmp_text_buff);
                    }
                    tmp_text_buff.Clear();
                    tmp_text_buff.Append(ch);
                }
                else if (tmp_text_buff.Length < 3)
                {
                    tmp_text_buff.Append(ch);
                }
            }
            else
            {
                if (tmp_text_buff.Length > 0 && tmp_text_buff[0] == ' ')
                {
                    decoded_text_buff.Append(morse_dict[tmp_text_buff.ToString()]);
                    tmp_text_buff.Clear();
                }
                tmp_text_buff.Append(ch);
            }
        }
        try
        {
            char? tmp_ch = morse_dict[tmp_text_buff.ToString()];
            Utilities.SwitchToValid(ref is_valid, decoded_text_buff);
            decoded_text_buff.Append(tmp_ch);
        }
        catch (KeyNotFoundException)
        {
            Utilities.SwitchToInvalid(ref is_valid, decoded_text_buff);
            decoded_text_buff.Append(tmp_text_buff);
        }
        if (!is_valid)
        {
            decoded_text_buff.Append('⁆');
        }
        return decoded_text_buff.ToString();
    }
}
