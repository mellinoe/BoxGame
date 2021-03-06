﻿using System.Runtime.InteropServices;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;
using System;

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
            Color = color;
            Normal = Vector3.Normalize(normal);
            Position = position;
            TextureCoords = textureCoords;
        }

        public unsafe Vector4 GetPositionAsVector4()
            => new Vector4(Position.X, Position.Y, Position.Z, 1);

        // Fields denoting the relative offsets of each component (from the structure's base)

        public const int PositionOffset = 0;
        public const int NormalOffset = 12;
        public const int ColorOffset = 24;
        public const int TexCoordOffset = 40;

        public static unsafe int SizeInBytes => sizeof(SimpleVertex);

        public static readonly SharpDX.Direct3D11.InputElement[] VertexInputLayout = new SharpDX.Direct3D11.InputElement[]
        {
            new SharpDX.Direct3D11.InputElement("Position",     0,  SharpDX.DXGI.Format.R32G32B32A32_Float,     PositionOffset,     0),
            new SharpDX.Direct3D11.InputElement("Normal",       0,  SharpDX.DXGI.Format.R32G32B32_Float,        NormalOffset,       0),
            new SharpDX.Direct3D11.InputElement("Color",        0,  SharpDX.DXGI.Format.R32G32B32A32_Float,     ColorOffset,        0),
            new SharpDX.Direct3D11.InputElement("TexCoord",     0,  SharpDX.DXGI.Format.R32G32_Float,           TexCoordOffset,     0)
        };
    }
}