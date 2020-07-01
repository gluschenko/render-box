using System;

namespace PathTracerSharp.Core
{
    public static class ColorHelpers
    {
        /* Original source: 
         * https://stackoverflow.com/questions/1335426/is-there-a-built-in-c-net-system-api-for-hsv-to-rgb 
         * https://www.programmingalgorithms.com/algorithm/rgb-to-hsv/ 
         */
        /// <summary>
        /// Returns HSV model of color
        /// </summary>
        public static void ToHSV(this Color color, out double hue, out double saturation, out double value)
        {
            double delta, min;
            double h = 0, s, v;

            min = Math.Min(Math.Min(color.R, color.G), color.B);
            v = Math.Max(Math.Max(color.R, color.G), color.B);
            delta = v - min;

            if (v == 0.0)
                s = 0;
            else
                s = delta / v;

            if (s == 0)
            {
                h = 0.0;
            }
            else
            {
                if (color.R == v)
                    h = (color.G - color.B) / delta;
                else if (color.G == v)
                    h = 2 + (color.B - color.R) / delta;
                else if (color.B == v)
                    h = 4 + (color.R - color.G) / delta;

                h *= 60;

                if (h < 0.0)
                    h += 360;
            }
            //
            hue = h;
            saturation = s;
            value = v;
        }

        /// <summary>
        /// Makes color from HSV
        /// </summary>
        public static Color FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            //value *= 255;
            float v = Convert.ToSingle(value);
            float p = Convert.ToSingle(value * (1 - saturation));
            float q = Convert.ToSingle(value * (1 - f * saturation));
            float t = Convert.ToSingle(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return new Color(v, t, p);
            else if (hi == 1)
                return new Color(q, v, p);
            else if (hi == 2)
                return new Color(p, v, t);
            else if (hi == 3)
                return new Color(p, q, v);
            else if (hi == 4)
                return new Color(t, p, v);
            else
                return new Color(v, p, q);
        }
    }
}
