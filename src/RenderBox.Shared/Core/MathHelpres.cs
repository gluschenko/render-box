﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RenderBox.Core
{
    public class MathHelpres
    {
        const double deg2rad = Math.PI / 180.0;
        const double rad2deg = 180.0 / Math.PI;
        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double DegToRad(double deg) => deg * deg2rad;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double RadToDeg(double rad) => rad * rad2deg;
        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double Clamp(double val, double low, double high) => Math.Max(low, Math.Min(high, val));

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double Clamp(double val) => Math.Max(0, Math.Min(1, val));
        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float Clamp(float val, float low, float high) => Math.Max(low, Math.Min(high, val));

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float Clamp(float val) => Math.Max(0, Math.Min(1, val));
        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static int Clamp(int val, int low, int high) => Math.Max(low, Math.Min(high, val));

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static int Clamp(int val) => Math.Max(0, Math.Min(1, val));
        /**/

        /// <summary>
        /// Линейная интерполяция
        /// </summary>
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double Lerp(double a, double b, double r) => a + (b - a) * r;

        /// <summary>
        /// Линейная интерполяция
        /// </summary>
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static int Lerp(int a, int b, double r) => (int)(a + (b - a) * r);

        /// <summary>
        /// Линейная интерполяция
        /// </summary>
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static long Lerp(long a, long b, double r) => (long)(a + (b - a) * r);

        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static bool SolveQuadratic(ref double a, ref double b, ref double c, ref double x0, ref double x1)
        {
            var discr = b * b - 4 * a * c;
            if (discr < 0)
                return false;
            else if (discr == 0)
                x0 = x1 = -0.5 * b / a;
            else
            {
                var q = (b > 0) ?
                    -0.5 * (b + FastSqrt(discr)) :
                    -0.5 * (b - FastSqrt(discr));
                x0 = q / a;
                x1 = c / q;
            }
            if (x0 > x1) (x0, x1) = (x1, x0);
            return true;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float QuakeInvSqrt(float number)
        {
            var half = 0.5f * number;
            var i = BitConverter.SingleToInt32Bits(number);
            i = 0x5F3759DF - (i >> 1);
            number = BitConverter.Int32BitsToSingle(i);
            number *= 1.5f - half * number * number;
            return number;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float FastSqrt(float number) => 1f / QuakeInvSqrt(number);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double QuakeInvSqrt(double number)
        {
            var half = 0.5 * number;
            var i = BitConverter.DoubleToInt64Bits(number);
            i = 0x5FE6EB50C7B537A9 - (i >> 1);
            number = BitConverter.Int64BitsToDouble(i);
            number *= 1.5 - half * number * number;
            return number;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double FastSqrt(double number) => 1.0 / QuakeInvSqrt(number);
    }
}
