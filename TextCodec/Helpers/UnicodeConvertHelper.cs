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

        /// <summary>
        /// 将字符 Unicode 码位值数组转换为特定进制数的字符串
        /// </summary>
        /// <param name="codepoints">码位数组</param>
        /// <param name="target_base">指定进制（2、8、10、16）</param>
        /// <returns>结果字符串</returns>
        internal static string CodepointsToValueStr(int[] codepoints, int target_base)
        {
            string[] strings = new string[codepoints.Length];
            int i = 0;
            foreach (int code_value in codepoints)
            {
                strings[i++] = Convert.ToString(code_value, target_base).ToUpper();
            }
            return string.Join(" ", strings);
        }

        /// <summary>
        /// 将码位字符串按空格或换行切割
        /// </summary>
        /// <param name="text">Unicode 码位字符串</param>
        /// <returns>Unicode 码位字符列表</returns>
        internal static string[] ValueStrToCodepointStrList(string text)
        {
            return text.Split(
                new char[] { ' ', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 将 Unicode 码位字符列表按指定进制转换为对应字符串，不合法码位加空格原样输出
        /// </summary>
        /// <param name="codepoint_strs">Unicode 码位字符列表</param>
        /// <param name="target_base">指定进制（2、8、10、16）</param>
        /// <returns>Unicode 字符串</returns>
        internal static string CodepointStrListToRawStr(string[] codepoint_strs, int target_base)
        {
            int code_value;
            string raw_text = string.Empty;
            foreach (var cp_str in codepoint_strs)
            {
                try
                {
                    code_value = Convert.ToInt32(cp_str, target_base);
                    raw_text += new Rune(code_value).ToString();
                }
                catch
                {
                    raw_text += ' ' + cp_str + ' ';
                }
            }
            return raw_text;
        }
    }
}
