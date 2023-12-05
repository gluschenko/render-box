using System.Runtime.CompilerServices;
using RenderBox.Shared.Core;

namespace RenderBox.Shared.Modules.Mandelbrot
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

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public virtual int GetInterationsCount(ComplexNumber c)
        {
            var i = 0;
            var z = new ComplexNumber();
            while (z.Norm() < MaxNorm && i < MaxIterations)
            {
                z = z * z + c;
                i++;
            }

            return i;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public virtual double Calc(ComplexNumber c, double defaultValue = 1)
        {
            int i = GetInterationsCount(c);

            if (i < MaxIterations)
            {
                return (double)i / MaxIterations;
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

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public ComplexNumber(double re, double im)
        {
            Re = re;
            Im = im;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public double Norm()
            => Re * Re + Im * Im;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static ComplexNumber operator +(ComplexNumber x, ComplexNumber y)
            => new(x.Re + y.Re, x.Im + y.Im);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static ComplexNumber operator *(ComplexNumber x, ComplexNumber y)
            => new(x.Re * y.Re - x.Im * y.Im, x.Re * y.Im + x.Im * y.Re);
    }
}
