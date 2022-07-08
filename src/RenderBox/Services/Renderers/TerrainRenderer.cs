using RenderBox.Core;
using RenderBox.Services.Rendering;
using RenderBox.Shared.Modules.Perlin;

namespace RenderBox.Services.Renderers
{
    public class TerrainRenderer : Renderer
    {
        public PerlinNoise Perlin { get; private set; }

        public TerrainRenderer(Paint paint) : base(paint)
        {
            Perlin = new PerlinNoise(Rand.Int(1, 100));
        }

        protected override void RenderScreen(RenderContext context)
        {
            var zoom = context.Width / 800f;
            var halfX = context.Width / 2f;
            var halfY = context.Height / 2f;

            Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                var tile = new Color[sizeX, sizeY];

                for (var y = 0; y < sizeY; y++)
                {
                    var posY = (iy + y) * zoom;

                    for (var x = 0; x < sizeX; x++)
                    {
                        var posX = (ix + x) * zoom;

                        var n = Perlin.FractalNoise2D(posX, posY, 4, 100, 1);

                        var color = ColorHelpers.FromHSV(360.0 * ((n + 1) / 2.0), 1, 1);

                        tile[x, y] = color;
                    }
                }

                return tile;
            }

            BatchScreen(context, Batch);
        }
    }
}
