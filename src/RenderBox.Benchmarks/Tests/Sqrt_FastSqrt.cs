using BenchmarkDotNet.Attributes;
using RenderBox.Core;
using System;

namespace RenderBox.Benchmarks.Tests
{
    public class Sqrt_FastSqrt
    {
        private readonly float _single;
        private readonly double _double;

        public Sqrt_FastSqrt()
        {
            _single = Math.Abs(Rand.Float()) * 10000f;
            _double = _single;
        }

        [Benchmark]
        public float Sqrt32() => MathF.Sqrt(_single);

        [Benchmark]
        public float InvSqrt32() => 1f / MathF.Sqrt(_single);

        [Benchmark]
        public float FastSqrt32() => MathHelpres.FastSqrt(_single);

        [Benchmark]
        public float FastInvSqrt32() => MathHelpres.QuakeInvSqrt(_single);

        [Benchmark]
        public double Sqrt64() => Math.Sqrt(_double);

        [Benchmark]
        public double InvSqrt64() => 1f / Math.Sqrt(_double);

        [Benchmark]
        public double FastSqrt64() => MathHelpres.FastSqrt(_double);

        [Benchmark]
        public double FastInvSqrt64() => MathHelpres.QuakeInvSqrt(_double);
    }
}
