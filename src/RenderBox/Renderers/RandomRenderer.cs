using RenderBox.Core;
using RenderBox.Rendering;
using System;

namespace RenderBox.Renderers
{
    public class RandomRenderer : Renderer
    {
        public RandomRenderer(Paint paint) : base(paint)
        {
        }

        protected override void RenderScreen(RenderContext context)
        {
            static Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                Color[,] tile = new Color[sizeX, sizeY];

                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        var r = Math.Max(0, Rand.Float());
                        var g = Math.Max(0, Rand.Float());
                        var b = Math.Max(0, Rand.Float());
                        tile[x, y] = new Color(r, g, b);
                    }
                }

                return tile;
            }

            BatchScreen(context, Batch);
        }
    }
}
