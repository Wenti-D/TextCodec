using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Core;

public enum HashMode : byte
{
    MD5,
    SHA1,
    SHA256,
    SHA512,
}
