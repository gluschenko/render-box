using System;

namespace RenderBox.Core
{
    public static class Rand
    {
        private static Random _random = new(RandomizeSeed());

        public static void InitState(int seed)
        {
            _random = null;
            _random = new Random(seed);
        }

        public static int Int(int min, int max) => MathHelpres.Lerp(min, max, Double()); //_random.Next(min, max);
        public static int Int() => Int(int.MinValue, int.MaxValue);

        public static double Double(double min, double max) => MathHelpres.Lerp(min, max, Double());
        public static double Double() => (double)FastNext() / int.MaxValue;  //random.NextDouble();

        public static float Float() => (float)FastNext() / int.MaxValue; //random.NextFloat();

        public static long Long(long min, long max) => MathHelpres.Lerp(min, max, Double());
        public static long Long() => Long(long.MinValue, long.MaxValue);

        public static int RandomizeSeed() => Guid.NewGuid().GetHashCode();
        public static void Reset() => _random = new Random(RandomizeSeed());

        private static int
            _x = RandomizeSeed(),
            _y = RandomizeSeed(),
            _z = RandomizeSeed(),
            _w = RandomizeSeed();

        private static int FastNext()
        {
            var t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));
        }
    }
}

