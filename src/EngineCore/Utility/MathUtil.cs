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
                0, 0, zNear / (zNear - zFar), 1);
        }

        public static Vector3 GetEulerAngles(this Quaternion q)
        {
            float attitude = (float)Math.Asin(2 * q.X * q.Y + 2 * q.Z * q.W);
            float heading;
            float bank;

            double edgeCase = q.X * q.Y + q.Z * q.W;
            if (edgeCase == 0.5)
            {
                heading = (float)(2 * Math.Atan2(q.X, q.W));
                bank = 0;
            }
            else if (edgeCase == -0.5)
            {
                heading = (float)(-2 * Math.Atan2(q.X, q.W));
                bank = 0;
            }
            else
            {
                heading = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, 1 - 2 * q.Y * q.Y - 2 * q.Z * q.Z);
                bank = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, 1 - 2 * q.X * q.X - 2 * q.Z * q.Z);
            }

            return new Vector3(bank, heading, attitude);
        }

        public static float RadiansToDegrees(float radians)
        {
            return (float)(radians * 180 / Math.PI);
        }

        public static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        public static Vector3 RadiansToDegrees(Vector3 radians)
        {
            return new Vector3(
                (float)(radians.X * 180 / Math.PI),
                (float)(radians.Y * 180 / Math.PI),
                (float)(radians.Z * 180 / Math.PI));
        }

        public static Vector3 DegreesToRadians(Vector3 degrees)
        {
            return new Vector3(
               (float)(degrees.X * Math.PI / 180),
               (float)(degrees.Y * Math.PI / 180),
               (float)(degrees.Z * Math.PI / 180));
        }
    }
}
