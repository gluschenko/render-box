using BenchmarkDotNet.Attributes;
using RenderBox.Core;
using System;

namespace RenderBox.Benchmarks.Tests
{
    public class Pow
    {
        private readonly float _single1;
        private readonly float _single2;
        private readonly double _double1;
        private readonly double _double2;

        public Pow()
        {
            _single1 = Math.Abs(Rand.Float()) * 100f;
            _single2 = Math.Abs(Rand.Float()) * 100f;
            _double1 = _single1;
            _double2 = _single2;
        }

        [Benchmark]
        public float Pow32() => MathF.Pow(_single1, _single2);

        [Benchmark]
        public double Pow64() => Math.Pow(_double1, _double2);

        [Benchmark]
        public float FastPow32() => MathHelpres.FastPow(_single1, _single2);

        [Benchmark]
        public double FastPow64() => MathHelpres.FastPow(_double1, _double2);

        [Benchmark]
        public double VeryFastPow64() => MathHelpres.VeryFastPow(_double1, _double2);
    }
}
