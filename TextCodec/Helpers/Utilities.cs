using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Helpers
{
    class Utilities
    {
        public static bool IsHexChar(char ch)
        {
            return ch is
                >= '0' and <= '9' or
                >= 'a' and <= 'f' or
                >= 'A' and <= 'F';
        }
    }
}
