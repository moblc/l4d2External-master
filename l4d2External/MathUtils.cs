using System;
using System.Numerics;

namespace left4dead2Menu
{
    internal static class MathUtils
    {
        public static float CalculateMagnitude(Vector3 from, Vector3 destination)
        {
            return (float)Math.Sqrt(
                Math.Pow(destination.X - from.X, 2) +
                Math.Pow(destination.Y - from.Y, 2) +
                Math.Pow(destination.Z - from.Z, 2));
        }

        public static float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        public static float Lerp(float start, float end, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount)); // Clamp amount
            return start + amount * NormalizeAngle(end - start); // 为角度归一化
        }
    }
}