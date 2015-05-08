using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EngineCore.Graphics.OpenGL
{
    public static class GLEx
    {
        public static void Color4f(Color4f color)
        {
            GL.Color4(color.R, color.G, color.B, color.A);
        }

        public static void Vertex3(Vector3 vector3)
        {
            GL.Vertex3(vector3.X, vector3.Y, vector3.Z);
        }

        public static void Normal3(Vector3 vector3)
        {
            GL.Normal3(vector3.X, vector3.Y, vector3.Z);
        }

        public unsafe static void LoadMatrix(ref Matrix4x4 matrix)
        {
            fixed (float* ptr = &matrix.M11)
            {
                GL.LoadMatrix(ptr);
            }
        }

        public static void LoadMatrix(ref OpenTK.Matrix4 mat)
        {
            GL.LoadMatrix(ref mat);
        }

        public static unsafe Matrix4x4 ToMatrix4x4(this OpenTK.Matrix4 mat)
        {
            return *(Matrix4x4*)&mat;
        }
    }
}
