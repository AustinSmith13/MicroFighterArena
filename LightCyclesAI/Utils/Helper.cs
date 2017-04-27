using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LightCyclesAI.Utils
{
    static class Helper
    {
        // Scale factor for the PCB
        public const float ScaleFactor = 5000;

        /// <summary>
        /// Sets it facing towards camera.
        /// </summary>
        public static Matrix4 BillboardMatrix(ref Matrix4 modelViewMat)
        {
            Vector3 trans = modelViewMat.ExtractTranslation();
            Vector3 scale = modelViewMat.ExtractScale();
            return new Matrix4(
                scale.X, 0f, 0f, 0f,
                0f, scale.Y, 0f, 0f,
                0f, 0f, scale.Z, 0f,
                trans.X, trans.Y, trans.Z, 1f);
        }

        /// <summary>
        /// Applies a rotation transformation to a vector.
        /// </summary>
        public static Vector3 PointRotationByQuaternion(Quaternion q1, Vector3 point)
        {
            Vector3 u = new Vector3(q1.X, q1.Y, q1.Z);
            float s = q1.W;
            return 2.0f * Vector3.Dot(u, point) * u + (s * s - Vector3.Dot(u, u)) * point + 2.0f * s * Vector3.Cross(u, point);

        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegreeToRadian(float angleInDegrees)
        {
            return (float)Math.PI * angleInDegrees / 180.0f;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadianToDegree(float angleInRadians)
        {
            return (angleInRadians * 180.0f) / (float)Math.PI;
        }
    }
}
