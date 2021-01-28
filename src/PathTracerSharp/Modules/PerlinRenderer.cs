using PathTracerSharp.Core;
using PathTracerSharp.Rendering;
using PathTracerSharp.Shared.Modules.Perlin;

namespace PathTracerSharp.Modules
{
    public class PerlinRenderer : Renderer
    {
        public PerlinNoise Perlin { get; private set; }

        public PerlinRenderer(Paint paint) : base(paint)
        {
            Perlin = new PerlinNoise(Rand.Int(1, 100));
        }

        protected override void RenderScreen(RenderContext context)
        {
            float zoom = context.Width / 800f;
            float halfX = context.Width / 2;
            float halfY = context.Height / 2;

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

                        var color = n > 0 ? ColorHelpers.FromHSV(120.0, 1, n) : ColorHelpers.FromHSV(240.0, 1, -n);

                        tile[x, y] = color;
                    }
                }

                return tile;
            }

            BatchScreen(context, batch);
        }
    }
}
