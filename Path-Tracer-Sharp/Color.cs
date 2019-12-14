using System;

namespace PathTracerSharp
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

        public float r, g, b;

        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public int GetRaw()
        {
            return
                GetChannel(r) << 16 |
                GetChannel(g) << 8 |
                GetChannel(b);
        }

        private byte GetChannel(float n) 
        {
            if (n < 0) return 0;
            if (n > 1) return byte.MaxValue;

            return (byte)(n * byte.MaxValue);
        }

        public static Color operator +(Color a, Color b) => new Color(a.r + b.r, a.g + b.g, a.b + b.b);
        public static Color operator -(Color a, Color b) => new Color(a.r - b.r, a.g - b.g, a.b - b.b);
        public static Color operator *(Color a, Color b) => new Color(a.r * b.r, a.g * b.g, a.b * b.b);

        public static Color operator *(Color a, float m) => new Color(a.r * m, a.g * m, a.b * m);
        public static Color operator /(Color a, float d) => new Color(a.r / d, a.g / d, a.b / d);
    }
}
