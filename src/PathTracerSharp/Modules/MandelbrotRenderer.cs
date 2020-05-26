using PathTracerSharp.Core;
using PathTracerSharp.Modules.Mandelbrot;
using PathTracerSharp.Rendering;
using System;
using System.Windows.Input;

namespace PathTracerSharp.Modules
{
    public class MandelbrotRenderer : Renderer
    {
        public MandelbrotSet Mandelbrot { get; private set; }

        public double Zoom = 1;
        public float OffsetX = 0;
        public float OffsetY = 0;

        public int Iterations = 100;
        public double Extent = 2;

        public MandelbrotRenderer(Paint paint) : base(paint)
        {
            Mandelbrot = new MandelbrotSet(Iterations, Extent);
        }

        protected override void RenderRoutine(RenderContext context)
        {
            double zoom = (context.width / 3.0) * (1.0 / Zoom);
            double halfX = (context.width / 2) + (OffsetX * zoom);
            double halfY = (context.height / 2) + (OffsetY * zoom);

            Color[,] renderBatch(int ix, int iy, int sizeX, int sizeY, int step)
            {
                Color[,] tile = new Color[sizeX, sizeY];

                for (int localY = 0; localY < sizeY; localY += step)
                {
                    int globalY = iy + localY;
                    double posY = (globalY - halfY) / zoom;

                    for (int localX = 0; localX < sizeX; localX += step)
                    {
                        int globalX = ix + localX;
                        double posX = (globalX - halfX) / zoom - 0.5;

                        //
                        var n = (float)Mandelbrot.Calc(new ComplexNumber(posX, posY), 1);
                        tile[localX, localY] = new Color(n, n, n); //ColorHelpers.FromHSV(120.0, 1, n);
                    }
                }

                return tile;
            }

            Color[,] batch(int ix, int iy, int sizeX, int sizeY) 
                => renderBatch(ix, iy, sizeX, sizeY, 1);

            Color[,] batchPreview(int ix, int iy, int sizeX, int sizeY) 
                => renderBatch(ix, iy, sizeX, sizeY, 4);

            BatchScreen(context, batchPreview);

            BatchScreen(context, batch);
        }

        public override void OnKeyPress(Key key, Action onRender)
        {
            bool wasd = key == Key.W || key == Key.A || key == Key.S || key == Key.D;

            if (key == Key.W) OffsetY += (float)(Zoom / 20);
            if (key == Key.S) OffsetY -= (float)(Zoom / 20);
            if (key == Key.A) OffsetX += (float)(Zoom / 20);
            if (key == Key.D) OffsetX -= (float)(Zoom / 20);

            if (wasd) 
            {
                onRender();
            }

            bool zoom = key == Key.Q || key == Key.E;

            if (zoom) 
            {
                if (key == Key.Q) ZoomOut();
                if (key == Key.E) ZoomIn();

                onRender();
            }
        }

        public void ZoomIn()
        {
            Zoom -= Zoom / 10;
        }

        public void ZoomOut()
        {
            Zoom += Zoom / 10;
        }
    }
}
