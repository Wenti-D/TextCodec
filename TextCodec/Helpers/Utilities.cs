using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Helpers;

static class Utilities
{
    public static bool IsHexChar(char ch)
    {
        return ch is
            >= '0' and <= '9' or
            >= 'a' and <= 'f' or
            >= 'A' and <= 'F';
    }

    public static bool IsTargetBaseChar(char ch, int target_base)
    {
        int shift;
        if (target_base >= 2 && target_base <= 10)
        {
            shift = target_base - 1;
            return ch >= '0' && ch <= (shift + '0');
        }
        else if (target_base > 10 && target_base <= 36)
        {
            shift = target_base - 11;
            return ch >= '0' && ch <= '9'
                || ch >= 'a' && ch <= 'a' + shift
                || ch >= 'A' && ch <= 'A' + shift;
        }
        return false;
    }
}
