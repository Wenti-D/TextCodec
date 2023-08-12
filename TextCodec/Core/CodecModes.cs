﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.Core;

public enum CodecMode : byte
{
    [Description("什么也不干")]
    None,

    [Description("转换为Unicode码位（二进制）")]
    UnicodeBin,
    [Description("转换为Unicode码位（八进制）")]
    UnicodeOct,
    [Description("转换为Unicode码位（十进制）")]
    UnicodeDec,
    [Description("转换为Unicode码位（十六进制）")]
    UnicodeHex,

    [Description("转换为十六进制 UTF-8 编码")]
    UTF8,
    [Description("转换为十六进制 UTF-16 小端序编码")]
    UTF16LE,
    [Description("转换为十六进制 UTF-16 大端序编码")]
    UTF16BE,
}
