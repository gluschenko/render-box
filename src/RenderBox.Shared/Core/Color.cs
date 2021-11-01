using System.Runtime.CompilerServices;

namespace RenderBox.Core
{
    public struct Color
    {
        public static Color Black => new Color(0, 0, 0);
        public static Color White => new Color(1, 1, 1);
        public static Color Red => new Color(1, 0, 0);
        public static Color Yellow => new Color(1, 1, 0);
        public static Color Green => new Color(0, 1, 0);
        public static Color Cyan => new Color(0, 1, 1);
        public static Color Blue => new Color(0, 0, 1);
        public static Color Gray => new Color(0.5, 0.5, 0.5);

        //

        public float r, g, b, a;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Color(float r, float g, float b, float a = 1)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Color(double r, double g, double b, double a = 1)
        {
            this.r = (float)r;
            this.g = (float)g;
            this.b = (float)b;
            this.a = (float)a;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public void Clamp()
        {
            r = MathHelpres.Clamp(r);
            g = MathHelpres.Clamp(g);
            b = MathHelpres.Clamp(b);
            a = MathHelpres.Clamp(a);
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public int GetRaw()
        {
            return GetChannel(r) << 16 | GetChannel(g) << 8 | GetChannel(b);
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        private byte GetChannel(float n)
        {
            if (n < 0) return 0;
            if (n > 1) return byte.MaxValue;

            return (byte)(n * byte.MaxValue);
        }

        public static bool operator ==(Color a, Color b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        public static bool operator !=(Color a, Color b) => !(a == b);

        public static Color operator +(Color a, Color b) => new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        public static Color operator -(Color a, Color b) => new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
        public static Color operator *(Color a, Color b) => new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);

        public static Color operator *(Color a, float m) => new Color(a.r * m, a.g * m, a.b * m, a.a * m);
        public static Color operator /(Color a, float d) => new Color(a.r / d, a.g / d, a.b / d, a.a / d);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static explicit operator int(Color a) => a.GetRaw();

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public override int GetHashCode() => GetRaw();

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public override bool Equals(object obj)
        {
            if (obj is Color color)
            {
                return this == color;
            }
            return false;
        }

        public static Color Lerp(Color a, Color b, float r)
        {
            return a + (b - a) * r;
        }
    }
}
