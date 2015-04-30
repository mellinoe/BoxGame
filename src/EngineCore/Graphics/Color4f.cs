using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace EngineCore.Graphics
{
    public struct Color4f
    {
        private Vector4 backingVector;
        public Color4f(float r, float g, float b, float a)
        {
            backingVector = new Vector4(r, g, b, a);
        }

        public float R { get { return backingVector.X; } }
        public float G { get { return backingVector.Y; } }
        public float B { get { return backingVector.Z; } }
        public float A { get { return backingVector.W; } }

        public static readonly Color4f Red = new Color4f(1, 0, 0, 1);
        public static readonly Color4f Green = new Color4f(0, 1, 0, 1);
        public static readonly Color4f Blue = new Color4f(0, 0, 1, 1);
        public static readonly Color4f Yellow = new Color4f(1, 1, 0, 1);
        public static readonly Color4f Grey = new Color4f(.25f, .25f, .25f, 1);
        public static readonly Color4f Cyan = new Color4f(0, 1, 1, 1);
        public static readonly Color4f White = new Color4f(1, 1, 1, 1);
        public static readonly Color4f Silver = new Color4f(0xC0 / 255f, 0xC0 / 255f, 0xC0 / 255f, 1);
        public static readonly Color4f Orange = new Color4f(0xFF / 255f, 0xA5 / 255f, 0x00 / 255f, 1);

        public Int32 ToRgba32()
        {
            uint value =
                (uint)(A * Byte.MaxValue) << 24 |
                (uint)(R * Byte.MaxValue) << 16 |
                (uint)(G * Byte.MaxValue) << 8 |
                (uint)(B * Byte.MaxValue);

            return unchecked((int)value);
        }
    }
}
