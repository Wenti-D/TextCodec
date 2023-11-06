using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCodec.Core.Dicts;

namespace TextCodec.Core
{
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
                    if (!is_valid)
                    {
                        encoded_text_buff.Append("⁆ " + morse_dict[ch]);
                    }
                    else
                    {
                        encoded_text_buff.Append(morse_dict[ch]);
                    }
                    is_valid = true;
                    encoded_text_buff.Append(' ');
                }
                catch (KeyNotFoundException)
                {
                    if (is_valid)
                    {
                        is_valid = false;
                        encoded_text_buff.Append(" ⁅");
                    }
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
                        if (!is_valid)
                        {
                            decoded_text_buff.Append("⁆ " + morse_dict[tmp_text_buff.ToString()]);
                        }
                        else
                        {
                            decoded_text_buff.Append(morse_dict[tmp_text_buff.ToString()]);
                        }
                        is_valid = true;
                    }
                    catch (KeyNotFoundException)
                    {
                        if (is_valid)
                        {
                            is_valid = false;
                            decoded_text_buff.Append(" ⁅");
                        }
                        decoded_text_buff.Append(tmp_text_buff);
                    }
                    if (is_valid)
                    {
                        is_valid = false;
                        decoded_text_buff.Append(" ⁅");
                    }
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
                            if (!is_valid)
                            {
                                decoded_text_buff.Append("⁆ " + morse_dict[tmp_text_buff.ToString()]);
                            }
                            else
                            {
                                decoded_text_buff.Append(morse_dict[tmp_text_buff.ToString()]);
                            }
                            is_valid = true;
                        }
                        catch(KeyNotFoundException)
                        {
                            if (is_valid)
                            {
                                is_valid = false;
                                decoded_text_buff.Append(" ⁅");
                            }
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
                if (!is_valid)
                {
                    decoded_text_buff.Append("⁆ " + morse_dict[tmp_text_buff.ToString()]);
                }
                else
                {
                    decoded_text_buff.Append(morse_dict[tmp_text_buff.ToString()]);
                }
                is_valid = true;
            }
            catch (KeyNotFoundException)
            {
                if (is_valid)
                {
                    is_valid = false;
                    decoded_text_buff.Append(" ⁅");
                }
                decoded_text_buff.Append(tmp_text_buff);
            }
            if (!is_valid)
            {
                decoded_text_buff.Append('⁆');
            }
            return decoded_text_buff.ToString();
        }
    }
}
