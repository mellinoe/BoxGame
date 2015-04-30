using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Utility
{
    public static class MathUtil
    {
        public static Vector3 NormalizedOrZero(this Vector3 vec)
        {
            float length = vec.Length();
            if (length < float.Epsilon)
            {
                return Vector3.Zero;
            }
            else
            {
                return vec / length;
            }
        }

        public static Matrix4x4 CreateLookAtLH(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            // Calculation from here:
            // http://msdn.microsoft.com/en-us/library/windows/desktop/bb281710%28v=vs.85%29.aspx

            var zaxis = Vector3.Normalize(cameraTarget - cameraPosition);
            var xaxis = Vector3.Normalize(Vector3.Cross(cameraUpVector, zaxis));
            var yaxis = Vector3.Cross(zaxis, xaxis);

            return new Matrix4x4(xaxis.X, yaxis.X, zaxis.X, 0,
                xaxis.Y, yaxis.Y, zaxis.Y, 0,
                xaxis.Z, yaxis.Z, zaxis.Z, 0,
                -Vector3.Dot(xaxis, cameraPosition), -Vector3.Dot(yaxis, cameraPosition),
                -Vector3.Dot(zaxis, cameraPosition), 1f);
        }

        public static Matrix4x4 CreatePerspectiveFovLH(float fieldOfViewY, float aspectRatio, float zNearPlane, float zFarPlane)
        {
            float yScale = 1f / (float)Math.Tan(fieldOfViewY * .5f);
            float xScale = yScale / aspectRatio;

            return new Matrix4x4(
                xScale, 0, 0, 0,
                0, yScale, 0, 0,
                0, 0, zFarPlane / (zFarPlane - zNearPlane), 1,
                0, 0, -zNearPlane * zFarPlane / (zFarPlane - zNearPlane), 0);
        }

        internal static Matrix4x4 CreateOrthographic(float width, float height, float zNear, float zFar)
        {
            return new Matrix4x4(
                2 / width, 0, 0, 0,
                0, 2 / height, 0, 0,
                0, 0, 1 / (zFar - zNear), 0,
                0,0, zNear / (zNear - zFar), 1);
        }
    }
}
