using BenchmarkDotNet.Attributes;
using RenderBox.Core;
using System;

namespace RenderBox.Benchmarks.Tests
{
    public class Sqrt_FastSqrt
    {
        private readonly float _number;

        public Sqrt_FastSqrt()
        {
            _number = new Random().Next(0, 10000);
        }

        [Benchmark]
        public float Add() => _number + _number;

        [Benchmark]
        public float Sqrt32() => MathF.Sqrt(_number);

        [Benchmark]
        public float InvSqrt32() => 1f / MathF.Sqrt(_number);

        [Benchmark]
        public float FastSqrt32() => MathHelpres.FastSqrt(_number);

        [Benchmark]
        public float FastInvSqrt32() => MathHelpres.QuakeInvSqrt(_number);
    }
}
