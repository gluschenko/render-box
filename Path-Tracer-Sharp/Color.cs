using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public float R, G, B;

        public Color(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public int GetRaw()
        {
            return
                (byte)(R * 255) << 16 |
                (byte)(G * 255) << 8 |
                (byte)(B * 255);
        }
    }
}
