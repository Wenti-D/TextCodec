using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Core
{
    class Converters
    {
        public static byte[] ListToByteArrayConverter(List<object> list)
        {
            byte[] result = new byte[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                result[i] = Convert.ToByte(list[i]);
            }
            return result;
        }
    }
}
