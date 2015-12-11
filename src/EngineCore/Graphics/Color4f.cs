using System;
using System.Numerics;
using System.Globalization;

namespace EngineCore.Graphics
{
    public struct Color4f
    {
        private Vector4 _backingVector;
        public Color4f(float r, float g, float b, float a)
        {
            _backingVector = new Vector4(r, g, b, a);
        }

        public float R => _backingVector.X;
        public float G => _backingVector.Y;
        public float B => _backingVector.Z;
        public float A => _backingVector.W;

        public static readonly Color4f Red = new Color4f(1, 0, 0, 1);
        public static readonly Color4f Green = new Color4f(0, 1, 0, 1);
        public static readonly Color4f Blue = new Color4f(0, 0, 1, 1);
        public static readonly Color4f Yellow = new Color4f(1, 1, 0, 1);
        public static readonly Color4f Grey = new Color4f(.25f, .25f, .25f, 1);
        public static readonly Color4f Cyan = new Color4f(0, 1, 1, 1);
        public static readonly Color4f White = new Color4f(1, 1, 1, 1);
        public static readonly Color4f Silver = FromHexString("C0C0C0FF", HexColorFormat.RGBA);
        public static readonly Color4f Orange = FromHexString("FFA500FF", HexColorFormat.RGBA);
        public static readonly Color4f CornflowerBlue = FromHexString("ff6495ed", HexColorFormat.ARGB);

        public static Color4f FromHexString(string hexColor, HexColorFormat format)
        {
            switch (format)
            {
                case HexColorFormat.ARGB:
                    {
                        if (hexColor.Length != 8)
                        {
                            throw new ArgumentException("ARGB format must have exacly 8 characters.");
                        }

                        float a = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber) / 255f;
                        float r = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber) / 255f;
                        float g = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber) / 255f;
                        float b = byte.Parse(hexColor.Substring(6, 2), NumberStyles.HexNumber) / 255f;

                        return new Color4f(r, g, b, a);
                    }
                case HexColorFormat.RGBA:
                    {
                        if (hexColor.Length != 8)
                        {
                            throw new ArgumentException("RGBA format must have exacly 8 characters.");
                        }

                        float r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber) / 255f;
                        float g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber) / 255f;
                        float b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber) / 255f;
                        float a = byte.Parse(hexColor.Substring(6, 2), NumberStyles.HexNumber) / 255f;

                        return new Color4f(r, g, b, a);
                    }
                case HexColorFormat.RGB:
                    {
                        if (hexColor.Length != 6)
                        {
                            throw new ArgumentException("RGB format must have exacly 6 characters.");
                        }
                        float r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber) / 255f;
                        float g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber) / 255f;
                        float b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber) / 255f;

                        return new Color4f(r, g, b, 1.0f);
                    }

                default:
                    throw new InvalidOperationException("Invalid format: " + format);
            }
        }

        public enum HexColorFormat
        {
            ARGB,
            RGBA,
            RGB
        }
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
