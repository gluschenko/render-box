using RenderBox.Core;
using System;
using System.Runtime.CompilerServices;

namespace RenderBox.Rendering
{
    public static class PaintExtensions
    {
        /// <summary>
        /// A naive line-drawing algorithm:
        /// https://en.wikipedia.org/wiki/Line_drawing_algorithm
        /// </summary>
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static void Line(this Paint paint, int x1, int y1, int x2, int y2, Color color)
        {
            var rawColor = color.GetRaw();

            var dx = x2 - x1;
            var dy = y2 - y1;

            for (var x = x1; x <= x2; x++)
            {
                var y = y1 + (dy * (x - x1) / dx);
                paint.SetPixel(x, y, rawColor);
            }
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static void FillRect(this Paint paint, int x, int y, int width, int height, Color color)
        {
            var startX = Math.Min(x, x + width);
            var startY = Math.Min(y, y + height);

            var endX = startX + Math.Abs(width);
            var endY = startY + Math.Abs(height);

            startX = Math.Max(startX, 0);
            startY = Math.Max(startY, 0);

            endX = Math.Min(endX, paint.Width - 1);
            endY = Math.Min(endY, paint.Height - 1);

            var colors = new Color[endX - startX, endY - startY];
            colors.FillColors(color);

            paint.SetPixels(startX, startY, colors);
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static void FillColors(this Color[,] colors, Color color)
        {
            for (var x = 0; x < colors.GetLength(0); x++)
            {
                for (var y = 0; y < colors.GetLength(1); y++)
                {
                    colors[x, y] = color;
                }
            }
        }
    }
}
