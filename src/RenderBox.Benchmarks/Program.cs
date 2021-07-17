using BenchmarkDotNet.Running;
using RenderBox.Benchmarks.Tests;
using System;
using RenderBox.Core;

namespace RenderBox.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 100; i++)
            {
                var num = i / 10f;

                var a = MathF.Sqrt(num);
                var b = MathHelpres.FastSqrt(num);

                Console.WriteLine($"{num}: {a} vs {b}");
            }

            //BenchmarkRunner.Run<Md5_Sha256>();
            BenchmarkRunner.Run<Sqrt_FastSqrt>();

            Console.ReadKey();
        }
    }
}
