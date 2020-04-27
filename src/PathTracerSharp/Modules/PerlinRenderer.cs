using System;
using System.Windows.Threading;
using PathTracerSharp.Rendering;
using PathTracerSharp.Modules.Perlin;
using PathTracerSharp.Core;

namespace PathTracerSharp.Modules
{
    public class PerlinRenderer : Renderer
    {
        public PerlinNoise Perlin { get; private set; }

        public PerlinRenderer(Paint paint) : base(paint)
        {
            Perlin = new PerlinNoise(Rand.Int(1, 100));
        }

        protected override void RenderRoutine(RenderContext context)
        {
            float zoom = context.width / 800f;
            float halfX = context.width / 2;
            float halfY = context.height / 2;

            Color[,] batch(int ix, int iy, int sizeX, int sizeY)
            {
                Color[,] tile = new Color[sizeX, sizeY];

                for (int y = 0; y < sizeY; y++)
                {
                    float posY = (iy + y) * zoom;

                    for (int x = 0; x < sizeX; x++)
                    {
                        float posX = (ix + x) * zoom;
                        //
                        var n = Perlin.FractalNoise2D(posX, posY, 4, 100, 1);
                        var color = ColorHelpers.FromHSV(120.0, 1, n);
                        tile[x, y] = color;
                    }
                }

                return tile;
            }

            BatchScreen(context, batch);
        }
    }
}
