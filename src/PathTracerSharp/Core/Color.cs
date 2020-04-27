using System;

namespace PathTracerSharp.Core
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

        //

        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color(float r, float g, float b, float a = 1)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public void Clamp() 
        {
            R = MathHelpres.Clamp(R);
            G = MathHelpres.Clamp(G);
            B = MathHelpres.Clamp(B);
            A = MathHelpres.Clamp(A);
        }

        public int GetRaw()
        {
            return
                GetChannel(R) << 16 |
                GetChannel(G) << 8 |
                GetChannel(B);
        }

        private byte GetChannel(float n) 
        {
            if (n < 0) return 0;
            if (n > 1) return byte.MaxValue;

            return (byte)(n * byte.MaxValue);
        }

        public static bool operator ==(Color a, Color b) => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        public static bool operator !=(Color a, Color b) => !(a == b);

        public static Color operator +(Color a, Color b) => new Color(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
        public static Color operator -(Color a, Color b) => new Color(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
        public static Color operator *(Color a, Color b) => new Color(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);

        public static Color operator *(Color a, float m) => new Color(a.R * m, a.G * m, a.B * m, a.A * m);
        public static Color operator /(Color a, float d) => new Color(a.R / d, a.G / d, a.B / d, a.A / d);

        public static explicit operator int(Color a) => a.GetRaw();

        public override int GetHashCode() => GetRaw();

        public override bool Equals(object obj)
        {
            if (obj is Color color)
            {
                return this == color;
            }
            return false;
        }
    }
}
