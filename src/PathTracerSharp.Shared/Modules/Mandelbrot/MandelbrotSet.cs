using System.Runtime.CompilerServices;

namespace PathTracerSharp.Shared.Modules.Mandelbrot
{
    public class MandelbrotSet
    {
        public double MaxValueExtent { get; set; }
        public int MaxIterations { get; set; }
        public double MaxNorm { get; }

        public MandelbrotSet(int maxIterations = 100, double maxValueExtent = 2.0)
        {
            MaxValueExtent = maxValueExtent;
            MaxIterations = maxIterations;
            MaxNorm = MaxValueExtent * MaxValueExtent;
        }

        public double Calc(ComplexNumber c, double defaultValue = 1)
        {
            int i = 0;
            ComplexNumber z = new ComplexNumber();
            while (z.Norm() < MaxNorm && i < MaxIterations)
            {
                z = z * z + c;
                i++;
            }

            if (i < MaxIterations)
            {
                return (double) i / MaxIterations;
            }
            else
            {
                return defaultValue; // black
            }
        }
    }

    public struct ComplexNumber
    {
        public double Re, Im;
        public ComplexNumber(double re, double im) { Re = re; Im = im; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Norm() => Re * Re + Im * Im;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexNumber operator +(ComplexNumber x, ComplexNumber y) =>
            new ComplexNumber(x.Re + y.Re, x.Im + y.Im);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexNumber operator *(ComplexNumber x, ComplexNumber y) =>
            new ComplexNumber(x.Re * y.Re - x.Im * y.Im, x.Re * y.Im + x.Im * y.Re);
    }
}
