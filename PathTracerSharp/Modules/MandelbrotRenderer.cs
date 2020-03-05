using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using PathTracerSharp.Rendering;
using PathTracerSharp.Modules.Mandelbrot;
using PathTracerSharp.Core;

namespace PathTracerSharp.Modules
{
    public class MandelbrotRenderer : Renderer
    {
        public MandelbrotSet Mandelbrot { get; private set; }

        public int Iterations = 100;
        public double Extent = 2;

        public MandelbrotRenderer(Paint paint) : base(paint)
        {
            Mandelbrot = new MandelbrotSet(Iterations, Extent);
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
                        var n = Mandelbrot.Calc(new ComplexNumber(posX, posY), 1);
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
