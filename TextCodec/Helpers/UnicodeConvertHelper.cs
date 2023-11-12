using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Helpers
{
    class UnicodeConvertHelper
    {
        /// <summary>
        /// 将文本转换为字符 Unicode 码位值（不使用代理对）
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>码位数组</returns>
        internal static int[] RawStrToCodepoints(string text)
        {
            //text = text.Replace("\r\n", "\n").Replace("\r", "\n");
            var runes = text.Replace("\r\n", "\n").Replace("\r", "\n").EnumerateRunes();
            int[] codepoints = new int[runes.Count()];
            int i = 0;
            foreach (var rune in runes)
            {
                codepoints[i++] = rune.Value;
            }
            return codepoints;
        }

        /// <summary>
        /// 将字符 Unicode 码位值数组转换为特定进制数的字符串
        /// </summary>
        /// <param name="codepoints">码位数组</param>
        /// <param name="target_base">指定进制（2、8、10、16）</param>
        /// <returns>结果字符串</returns>
        internal static string CodepointsToValueStr(int[] codepoints, int target_base)
        {
            StringBuilder result_buff = new();
            foreach (int code_value in codepoints)
            {
                result_buff.Append(Convert.ToString(code_value, target_base).ToUpper());
                result_buff.Append(' ');
            }
            if (result_buff.Length > 0)
            {
                result_buff.Length--;
            }
            return result_buff.ToString();
        }

        /// <summary>
        /// 尝试解码在 tmp_buff 存储的码位字符串
        /// </summary>
        /// <param name="result_buff">结果字符串的 StringBuilder，引用</param>
        /// <param name="tmp_buff">临时存储码位字符串的 StringBuilder，引用</param>
        /// <param name="target_base">目标进制</param>
        /// <param name="is_valid">目前接收到的字符是否合法，引用</param>
        internal static void TryParseCodepointStr(ref StringBuilder result_buff, ref StringBuilder tmp_buff, int target_base, ref bool is_valid)
        {
            try
            {
                char _ = tmp_buff[0];
                Rune rune = new(Convert.ToInt32(tmp_buff.ToString(), target_base));
                if (!is_valid)
                {
                    is_valid = true;
                    result_buff.Append("⁆ ");
                }
                result_buff.Append(rune);
            }
            catch (IndexOutOfRangeException)
            {
                // 故意的
            }
            catch (Exception)
            {
                if (is_valid)
                {
                    result_buff.Append(" ⁅");
                    is_valid = false;
                }
                result_buff.Append(tmp_buff);
            }
            finally
            {
                tmp_buff.Clear();
            }
        }
    }
}
