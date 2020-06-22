using System;
using System.Windows.Input;
using PathTracerSharp.Core;
using PathTracerSharp.Options;
using PathTracerSharp.Pages;
using PathTracerSharp.Rendering;
using PathTracerSharp.Shared.Modules.Mandelbrot;

namespace PathTracerSharp.Modules
{
    [OptionsPage(typeof(MandelbrotPage))]
    public class MandelbrotRenderer : Renderer
    {
        public MandelbrotSet Mandelbrot { get; private set; }

        public double Zoom { get; set; } = 1;
        public float OffsetX { get; set; } = 0;
        public float OffsetY { get; set; } = 0;

        public int Iterations { get; set; } = 100;
        public double Extent { get; set; } = 2;

        public MandelbrotRenderer(Paint paint) : base(paint)
        {
            Mandelbrot = new MandelbrotSet();
        }

        protected override void RenderRoutine(RenderContext context)
        {
            Mandelbrot.SetIterations(Iterations);
            Mandelbrot.SetExtent(Extent);

            double zoom = (context.width / 3.0) * (1.0 / Zoom);
            double halfX = (context.width / 2) + (OffsetX * zoom);
            double halfY = (context.height / 2) + (OffsetY * zoom);

            var palette = new Color[Iterations + 1];
            for (int i = 0; i < palette.Length; i++) 
            {
                var n = (float)i / Iterations;
                palette[i] = new Color(n, n, n);
            }

            BatchScreen(context, batchPreview);

            BatchScreen(context, batch);

            //context.dispatcher.Invoke(() => Paint.FillRect(10, 10, 80, 40, Color.Green));

            //

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
                        var n = Mandelbrot.GetInterationsCount(new ComplexNumber(posX, posY));
                        tile[localX, localY] = palette[n];
                    }
                }

                return tile;
            }

            Color[,] batch(int ix, int iy, int sizeX, int sizeY)
            {
                return renderBatch(ix, iy, sizeX, sizeY, 1);
            }

            Color[,] batchPreview(int ix, int iy, int sizeX, int sizeY)
            {
                return renderBatch(ix, iy, sizeX, sizeY, 8);
            }
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
