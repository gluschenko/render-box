﻿using RenderBox.Shared.Core;

namespace RenderBox.Shared.Modules.Mandelbrot.Filters
{
    public class Filter1 : IPaletteFilter
    {
        public override Color GetColor(double rate, double zoom)
        {
            var r = MathHelpres.FastPow(rate, 3);
            var g = rate;
            var b = 0;

            return new Color((float)r, (float)g, (float)b);
        }
    }

    public class Filter2 : IPaletteFilter
    {
        public override Color GetColor(double rate, double zoom)
        {
            double contrast = 0.01 / (rate * rate + (0.02 * zoom));
            double color = MathHelpres.FastPow(rate, contrast);

            var r = color;
            var g = color * color;
            var b = 0;

            return new Color((float)r, (float)g, (float)b);
        }
    }

    public class Filter3 : IPaletteFilter
    {
        public override Color GetColor(double rate, double zoom)
        {
            double contrast = 0.01 / (rate * rate + (0.02 * zoom));
            double color = MathHelpres.FastPow(rate, contrast);

            var r = color * color * color;
            var g = color * color;
            var b = color;

            return new Color((float)r, (float)g, (float)b);
        }
    }

    public class Filter4 : IPaletteFilter
    {
        public override Color GetColor(double rate, double zoom)
        {
            double contrast = 0.01 / (rate * rate + (0.02 * zoom));
            double color = MathHelpres.FastPow(rate, contrast);

            color *= 2;

            double colorZero = color <= 1 ? color : 0;
            double colorUnit = color <= 1 ? color : 1;
            double colorDelta = color - colorUnit;

            var r = MathHelpres.FastPow((colorUnit - colorDelta) * (colorUnit + colorDelta), 1.5);
            var g = MathHelpres.FastPow((colorUnit - colorDelta) * (colorUnit + colorDelta), 2);
            var b = MathHelpres.FastPow((colorUnit - colorDelta) * (1 - colorDelta), 1);

            return new Color((float)r, (float)g, (float)b);
        }
    }

    public class Filter5 : IPaletteFilter
    {
        public override Color GetColor(double rate, double zoom)
        {
            double contrast = 0.01 / (rate * rate + (0.02 * zoom));
            double color = MathHelpres.FastPow(rate, contrast);

            double c = color * 10;

            var r = color * Math.Abs(Math.Sin(c));
            var g = color * Math.Abs(Math.Cos(c));
            var b = color;

            return new Color((float)r, (float)g, (float)b);
        }
    }

    public class Filter6 : IPaletteFilter
    {
        public override Color GetColor(double rate, double zoom)
        {
            return ColorHelpers.FromHSV(240, 1.0 - rate, 1);
        }
    }
}
