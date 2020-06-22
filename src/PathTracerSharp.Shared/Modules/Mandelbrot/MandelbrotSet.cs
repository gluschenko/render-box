using System.Runtime.CompilerServices;

namespace PathTracerSharp.Shared.Modules.Mandelbrot
{
    public class MandelbrotSet
    {
        public double MaxValueExtent { get; private set; }
        public int MaxIterations { get; private set; }
        public double MaxNorm { get; private set; }

        public MandelbrotSet(int maxIterations = 100, double maxValueExtent = 2.0)
        {
            SetIterations(maxIterations);
            SetExtent(maxValueExtent);
        }

        public void SetIterations(int maxIterations) 
        {
            MaxIterations = maxIterations;
        }

        public void SetExtent(double maxValueExtent) 
        {
            MaxValueExtent = maxValueExtent;
            MaxNorm = MaxValueExtent * MaxValueExtent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int GetInterationsCount(ComplexNumber c) 
        {
            int i = 0;
            ComplexNumber z = new ComplexNumber();
            while (z.Norm() < MaxNorm && i < MaxIterations)
            {
                z = z * z + c;
                i++;
            }

            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual double Calc(ComplexNumber c, double defaultValue = 1)
        {
            int i = GetInterationsCount(c);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
