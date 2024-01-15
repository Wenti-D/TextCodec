using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCodec.Helpers;

namespace TextCodec.Core.Codecs;

public class AlphabetOrdinary
{
    public static string Encoder(string rawText)
    {
        bool isValid = true;
        StringBuilder resultBuff = new();
        foreach (char ch in rawText)
        {
            int ord;
            // 大写拉丁字母
            if (ch is >= 'A' and <= 'Z')
            {
                ord = ch - 'A' + 1;
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append(ord);
            }
            // 小写拉丁字母
            else if (ch is >= 'a' and <= 'z')
            {
                ord = ch - 'a' + 1;
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append(ord);
            }
            // 大写希腊字母，U+03A2 没有字符，拆开处理
            else if (ch is >= 'Α' and <= 'Ρ')
            {
                ord = ch - 'Α' + 1;
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append(ord);
            }
            else if (ch is >= 'Σ' and <= 'Ω')
            {
                ord = ch - 'Σ' + 18;
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append(ord);
            }
            // 小写希腊字母，Σ 有两个小写，拆开处理
            else if (ch is >= 'α' and <= 'ς')
            {
                ord = ch - 'Α' + 1;
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append(ord);
            }
            else if (ch is >= 'σ' and <= 'ω')
            {
                ord = ch - 'σ' + 18;
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append(ord);
            }
            // 大写西里尔字母
            else if (ch is >= 'А' and <= 'Я')
            {
                ord = ch - 'А' + 1;
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append(ord);
            }
            // 小写西里尔字母
            else if (ch is >= 'а' and <= 'я')
            {
                ord = ch - 'а' + 1;
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append(ord);
            }
            else
            {
                if (isValid && resultBuff.Length > 0)
                {
                    resultBuff.Length--;
                }
                Utilities.SwitchToInvalid(ref isValid, resultBuff);
                resultBuff.Append(ch);
            }

            if (isValid)
            {
                resultBuff.Append(' ');
            }
        }
        if (!isValid)
        {
            resultBuff.Append('⁆');
        }
        return resultBuff.ToString();
    }

    public static string Decoder(string encodedText)
    {
        bool isValid = true;
        StringBuilder resultBuff = new(), tmpBuff = new();
        int ord;
        foreach (char ch in encodedText)
        {
            if (char.IsAsciiDigit(ch))
            {
                tmpBuff.Append(ch);
            }
            else if (tmpBuff.Length != 0)
            {
                try
                {
                    ord = int.Parse(tmpBuff.ToString());
                    if (ord >= 1 && ord <= 26)
                    {
                        Utilities.SwitchToValid(ref isValid, resultBuff);
                        resultBuff.Append((char)('A' + ord - 1));
                    }
                    else
                    {
                        Utilities.SwitchToInvalid(ref isValid, resultBuff);
                        resultBuff.Append(tmpBuff);
                    }
                }
                catch (OverflowException)
                {
                    Utilities.SwitchToInvalid(ref isValid, resultBuff);
                    resultBuff.Append(tmpBuff);
                }
                finally
                {
                    if (!char.IsWhiteSpace(ch) || !isValid)
                    {
                        Utilities.SwitchToInvalid(ref isValid, resultBuff);
                        resultBuff.Append(ch);
                    }
                    tmpBuff.Clear();
                }
            }
            else
            {
                Utilities.SwitchToInvalid(ref isValid, resultBuff);
                resultBuff.Append(ch);
            }
        }
        if (tmpBuff.Length != 0)
        {
            if (int.TryParse(tmpBuff.ToString(), out ord) && ord >= 1 && ord <= 26)
            {
                Utilities.SwitchToValid(ref isValid, resultBuff);
                resultBuff.Append((char)('A' + ord - 1));
            }
            else
            {
                Utilities.SwitchToInvalid(ref isValid, resultBuff);
                resultBuff.Append(tmpBuff);
            }
        }
        if (!isValid)
        {
            resultBuff.Append('⁆');
        }
        return resultBuff.ToString();
    }
}
