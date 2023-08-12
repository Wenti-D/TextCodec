﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCodec.Helpers;

namespace TextCodec.Core
{
    class UnicodeCodec
    {
        public static string BinEncoder(string rawText) => Encoder(rawText, 2);
        public static string BinDecoder(string text) => Decoder(text, 2);
        public static string OctEncoder(string rawText) => Encoder(rawText, 8);
        public static string OctDecoder(string text) => Decoder(text, 8);
        public static string DecEncoder(string rawText) => Encoder(rawText, 10);
        public static string DecDecoder(string text) => Decoder(text, 10);
        public static string HexEncoder(string rawText) => Encoder(rawText, 16);
        public static string HexDecoder(string text) => Decoder(text, 16);


        private static string Encoder(string rawText, int target_base)
        {
            int[] codepoints = UnicodeConvertHelper.RawStrToCodepoints(rawText);
            return UnicodeConvertHelper.CodepointsToValueStr(codepoints, target_base);
        }

        private static string Decoder(string text, int target_base)
        {
            string[] codepoint_strs = UnicodeConvertHelper.ValueStrToCodepointStrList(text);
            return UnicodeConvertHelper.CodepointStrListToRawStr(codepoint_strs, target_base);
        }
    }
}
