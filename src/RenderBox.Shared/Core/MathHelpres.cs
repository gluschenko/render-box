using System.Runtime.CompilerServices;

namespace RenderBox.Shared.Core
{
    public class MathHelpres
    {
        public const double DEG2RAD = Math.PI / 180.0;
        public const double RAD2DEG = 180.0 / Math.PI;

        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double DegToRad(double deg) => deg * DEG2RAD;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double RadToDeg(double rad) => rad * RAD2DEG;
        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double Clamp(double val, double low, double high) => Math.Clamp(val, low, high);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double Clamp(double val) => Math.Clamp(val, 0, 1);
        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float Clamp(float val, float low, float high) => Math.Clamp(val, low, high);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float Clamp(float val) => Math.Clamp(val, 0, 1);
        /**/
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static int Clamp(int val, int low, int high) => Math.Clamp(val, low, high);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static int Clamp(int val) => Math.Clamp(val, 0, 1);
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
                var q = b > 0 ?
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

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float FastPow(float a, float b)
        {
            var i = (int)(BitConverter.DoubleToInt64Bits(a) >> 32);
            var j = (int)(b * (i - 1072632447) + 1072632447);
            return (float)BitConverter.Int64BitsToDouble((long)j << 32);
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double FastPow(double a, double b)
        {
            var i = (int)(BitConverter.DoubleToInt64Bits(a) >> 32);
            var j = (int)(b * (i - 1072632447) + 1072632447);
            return BitConverter.Int64BitsToDouble((long)j << 32);
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static double VeryFastPow(double a, double b)
        {
            var i = BitConverter.DoubleToInt64Bits(a);
            var j = (long)(b * (i - 4606921280493453312L)) + 4606921280493453312L;
            return BitConverter.Int64BitsToDouble(j);
        }
    }
}
