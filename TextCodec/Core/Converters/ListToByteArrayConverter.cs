using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Core.Converters
{
    class ListToByteArrayConverter
    {
        public static byte[] Convert(List<object> list)
        {
            byte[] result = new byte[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                result[i] = System.Convert.ToByte(list[i]);
            }
            return result;
        }
    }
}
