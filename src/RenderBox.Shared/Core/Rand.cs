using System;

namespace RenderBox.Core
{
    public static class Rand
    {
        public static int Int(int min, int max) => MathHelpres.Lerp(min, max, Double());
        public static int Int() => Int(int.MinValue, int.MaxValue);

        public static double Double(double min, double max) => MathHelpres.Lerp(min, max, Double());
        public static double Double() => (double)Next() / int.MaxValue;

        public static float Float() => (float)Next() / int.MaxValue;

        public static long Long(long min, long max) => MathHelpres.Lerp(min, max, Double());
        public static long Long() => Long(long.MinValue, long.MaxValue);

        public static int RandomizeSeed() => Guid.NewGuid().GetHashCode();

        private static int
            _x = RandomizeSeed(),
            _y = RandomizeSeed(),
            _z = RandomizeSeed(),
            _w = RandomizeSeed();

        private static int Next()
        {
            var t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));
        }
    }
}

