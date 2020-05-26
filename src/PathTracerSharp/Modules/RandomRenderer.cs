using PathTracerSharp.Core;
using PathTracerSharp.Rendering;

namespace PathTracerSharp.Modules
{
    public class RandomRenderer : Renderer
    {
        public RandomRenderer(Paint paint) : base(paint)
        {
        }

        protected override void RenderRoutine(RenderContext context)
        {
            double zoom = context.width / 3.0;
            float halfX = context.width / 2;
            float halfY = context.height / 2;

            Color[,] batch(int ix, int iy, int sizeX, int sizeY)
            {
                Color[,] tile = new Color[sizeX, sizeY];

                for (int y = 0; y < sizeY; y++)
                {
                    int globalY = iy + y;
                    double posY = (globalY - halfY) / zoom;

                    for (int x = 0; x < sizeX; x++)
                    {
                        int globalX = ix + x;
                        double posX = (globalX - halfX) / zoom - 0.5;
                        //
                        tile[x, y] = new Color(Rand.Float(), Rand.Float(), Rand.Float());
                    }
                }

                return tile;
            }

            BatchScreen(context, batch);
        }
    }
}
