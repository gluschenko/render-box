using System;
using RenderBox.Core;
using RenderBox.Services.Rendering;

namespace RenderBox.Services.Renderers
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
                var tile = new Color[sizeX, sizeY];

                for (var y = 0; y < sizeY; y++)
                {
                    for (var x = 0; x < sizeX; x++)
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
