using System;

namespace PathTracerSharp.Core
{
    public static class Rand
    {
        private static Random random = new Random(RandomizeSeed());

        public static void InitState(int seed)
        {
            random = null;
            random = new Random(seed);
        }

        public static int Int(int min, int max) => random.Next(min, max);
        public static int Int() => Int(int.MinValue, int.MaxValue);

        public static double Double(double min, double max) => MathHelpres.Lerp(min, max, Double());
        public static double Double() => random.NextDouble();

        public static long Long(long min, long max) => MathHelpres.Lerp(min, max, Double());
        public static long Long() => Long(long.MinValue, long.MaxValue);

        /// <summary>
        /// Генерация начального состояния (seed) со значительно сниженной предсказуемостью
        /// </summary>
        public static int RandomizeSeed() => Guid.NewGuid().GetHashCode();
        /// <summary>
        /// Сброс состояния
        /// </summary>
        public static void Reset() => random = new Random(RandomizeSeed());
    }
}

