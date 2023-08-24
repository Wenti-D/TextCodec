using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;

namespace TextCodec.Core
{
    [StructLayout(LayoutKind.Explicit)]
    struct WindowRect
    {
        [FieldOffset(0)]
        public ulong val;
        [FieldOffset(0)]
        public short x;
        [FieldOffset(2)]
        public short y;
        [FieldOffset(4)]
        public short width;
        [FieldOffset(6)]
        public short height;

        public readonly int Left => x;
        public readonly int Top => y;
        public readonly int Right => x + width;
        public readonly int Bottom => y + height;

        public WindowRect(int X, int Y, int Width, int Height)
        {
            x = (short)X; y = (short)Y;
            width = (short)Width; height = (short)Height;
        }

        public WindowRect(ulong Value)
        {
            val = Value;
        }

        public RectInt32 ToRectInt32()
        {
            return new RectInt32(x, y, width, height);
        }
    }
}
