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
            CheckSqrt();
            CheckPow();

            BenchmarkRunner.Run<Pow>();
            BenchmarkRunner.Run<Sqrt_FastSqrt>();

            Console.ReadKey();
        }

        static void CheckSqrt()
        {
            Console.WriteLine("");
            Console.WriteLine("SQRT");
            Console.WriteLine("");

            for (var i = 0; i < 100; i++)
            {
                var num = i / 10f;

                var a = MathF.Sqrt(num);
                var b = MathHelpres.FastSqrt(num);

                Console.WriteLine($"{num}: {a} vs {b} (error {GetError(a, b)})");
            }

            for (var i = 0; i < 100; i++)
            {
                var num = i / 10.0;

                var a = Math.Sqrt(num);
                var b = MathHelpres.FastSqrt(num);

                Console.WriteLine($"{num}: {a} vs {b} (error {GetError(a, b)})");
            }
        }

        static void CheckPow()
        {
            Console.WriteLine("");
            Console.WriteLine("POW");
            Console.WriteLine("");

            for (var i = -50; i < 50; i++)
            {
                var num = Math.Abs(i / 20f);
                var num2 = i / 30f;

                var a = MathF.Pow(num, num2);
                var b = MathHelpres.FastPow(num, num2);

                Console.WriteLine($"{num,5} ^ {num2,5}: {a,20} vs {b,20} (error {GetError(a, b)})");
            }

            for (var i = -50; i < 50; i++)
            {
                var num = Math.Abs(i / 20.0);
                var num2 = i / 30.0;

                var a = Math.Pow(num, num2);
                var b = MathHelpres.FastPow(num, num2);

                Console.WriteLine($"{num,5} ^ {num2,5}: {a,20} vs {b,20} (error {GetError(a, b)})");
            }
        }

        static string GetError(double a, double b)
        {
            var max = Math.Max(a, b);
            var rate = max != 0 ? Math.Abs(a - b) / max : 0;
            return $"{Math.Round(rate * 100, 2)}%";
        }
    }
}
