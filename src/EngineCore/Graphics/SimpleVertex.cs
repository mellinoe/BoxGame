using System.Runtime.InteropServices;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;
using System;
#if FEATURE_SHARPDX
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
#endif

namespace EngineCore.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SimpleVertex
    {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public readonly Color4f Color;
        public readonly Vector2 TextureCoords;

        public SimpleVertex(Vector3 position, Vector3 normal, Color4f color, Vector2 textureCoords)
        {
            this.Color = color;
            this.Normal = Vector3.Normalize(normal);
            this.Position = position;
            this.TextureCoords = textureCoords;
        }

        public unsafe Vector4 GetPositionAsVector4()
        {
            return new Vector4(Position.X, Position.Y, Position.Z, 1);
        }

        // Fields for use in an InputLayout struct.

        public static readonly int PositionOffset = 0;
        public static readonly int NormalOffset = 12;
        public static readonly int ColorOffset = 24;
        public static readonly int TexCoordOffset = 40;

        public static unsafe int SizeInBytes { get { return sizeof(SimpleVertex); } }

#if FEATURE_SHARPDX
        public static readonly InputElement[] VertexInputLayout = new InputElement[]
        {
            new InputElement("Position",    0,      Format.R32G32B32A32_Float,  PositionOffset, 0),
            new InputElement("Normal",      0,      Format.R32G32B32_Float,     NormalOffset,   0),
            new InputElement("Color",       0,      Format.R32G32B32A32_Float,  ColorOffset,    0)
        };
#endif
    }
}