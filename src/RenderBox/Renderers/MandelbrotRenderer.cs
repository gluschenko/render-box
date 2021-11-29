using RenderBox.Core;
using RenderBox.Options;
using RenderBox.Pages;
using RenderBox.Rendering;
using RenderBox.Shared.Modules.Mandelbrot;
using RenderBox.Shared.Modules.Mandelbrot.Filters;
using System;
using System.Collections.Concurrent;
using System.Windows.Input;

namespace RenderBox.Renderers
{
    [OptionsPage(typeof(MandelbrotPage))]
    public class MandelbrotRenderer : Renderer
    {
        public MandelbrotSet Mandelbrot { get; private set; }

        public double Zoom { get; set; } = 1;
        public double OffsetX { get; set; } = 0;
        public double OffsetY { get; set; } = 0;

        public int Iterations { get; set; } = 100;
        public double Extent { get; set; } = 2;

        public IPaletteFilter Filter { get; set; }

        public MandelbrotRenderer(Paint paint) : base(paint)
        {
            Mandelbrot = new MandelbrotSet();
        }

        protected override void RenderScreen(RenderContext context)
        {
            Mandelbrot.SetIterations(Iterations);
            Mandelbrot.SetExtent(Extent);

            double scale = context.Scale;
            double zoom = (context.Width / 3.0) * (1.0 / Zoom);
            double halfX = (context.Width / 2.0) + (OffsetX * zoom);
            double halfY = (context.Height / 2.0) + (OffsetY * zoom);

            var rates = new ConcurrentDictionary<Point2, int>();
            var palette = new Color[Iterations + 1];

            for (int i = 0; i < palette.Length; i++)
            {
                var n = (double)i / Iterations;
                palette[i] = Filter is not null
                    ? Filter.GetColor(n, Zoom)
                    : new Color((float)n, (float)n, (float)n);
            }

            BatchScreen(context, BatchPreview);

            BatchScreen(context, Batch, GetRenderPriority);

            //

            Color[,] BatchPreview(int ix, int iy, int sizeX, int sizeY)
            {
                var tile = RenderBatch(ix, iy, sizeX, sizeY, (int)(8 * scale));
                rates[new Point2(ix, iy)] = CalcRate(tile);
                return Colorize(tile);
            }

            Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                var tile = RenderBatch(ix, iy, sizeX, sizeY, 1);
                return Colorize(tile);
            }

            int GetRenderPriority(int x, int y)
                => rates.TryGetValue(new Point2(x, y), out var rate) ? rate : 0;

            int[,] RenderBatch(int ix, int iy, int sizeX, int sizeY, int step)
            {
                var tile = new int[sizeX, sizeY];

                var cellPerPixel = 1.0 / zoom;

                var posX = ix - halfX;
                var posY = iy - halfY;

                posX *= cellPerPixel;
                posY *= cellPerPixel;

                posX -= 0.7;
                cellPerPixel *= step;

                for (int localY = 0; localY < sizeY; localY += step)
                {
                    var localPosX = posX;

                    posY += cellPerPixel;

                    for (int localX = 0; localX < sizeX; localX += step)
                    {
                        localPosX += cellPerPixel;
                        //
                        var c = new ComplexNumber(localPosX, posY);
                        tile[localX, localY] = Mandelbrot.GetInterationsCount(c);
                    }
                }

                if (step > 1)
                {
                    for (int localY = 0; localY < sizeY; localY += step)
                    {
                        for (int localX = 0; localX < sizeX; localX += step)
                        {
                            var template = tile[localX, localY];

                            for (var y = localY; y < localY + step && y < sizeY; y++)
                            {
                                for (var x = localX; x < localX + step && x < sizeX; x++)
                                {
                                    tile[x, y] = template;
                                }
                            }
                        }
                    }
                }

                return tile;
            }

            Color[,] Colorize(int[,] tile)
            {
                var width = tile.GetLength(0);
                var height = tile.GetLength(1);
                var colors = new Color[width, height];

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        colors[x, y] = palette[tile[x, y]];
                    }
                }

                return colors;
            }

            int CalcRate(int[,] tile)
            {
                var width = tile.GetLength(0);
                var height = tile.GetLength(1);
                var sum = 0;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        sum += tile[x, y];
                    }
                }

                return sum;
            }
        }

        public override void OnKeyPress(Key key, Action onRender)
        {
            bool wasd = key == Key.W || key == Key.A || key == Key.S || key == Key.D;

            if (key == Key.W) OffsetY += Zoom / 20;
            if (key == Key.S) OffsetY -= Zoom / 20;
            if (key == Key.A) OffsetX += Zoom / 20;
            if (key == Key.D) OffsetX -= Zoom / 20;

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
