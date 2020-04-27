using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using PathTracerSharp.Core;
using PathTracerSharp.Modules.PathTracer;
using System.Windows.Input;

namespace PathTracerSharp.Rendering
{
    public delegate void RenderStartHandler();
    public delegate void RenderCompleteHandler();

    /*public abstract class Renderer : Renderer<RenderContext>
    {
        public Renderer(Paint paint) : base(paint) { }
    }*/

    public abstract class Renderer : IDisposable //<T> where T : RenderContext
    {
        // public
        public int BatchSize { get; set; } = 32;
        public Paint Paint { get; set; }

        public event RenderStartHandler RenderStart;
        public event RenderCompleteHandler RenderComplete;
        // private
        private Thread renderThread;
        //
        public Renderer(Paint paint)
        {
            Paint = paint;
        }

        public void Dispose()
        {
            Stop();
            Paint.Dispose();
        }

        public virtual void OnKeyPress(Key key, Action onRender) { }

        public void Render(Dispatcher dispatcher) 
        {
            Render(BuildContext(dispatcher));
        }

        public void Render(RenderContext context)
        {
            Stop();

            renderThread = new Thread(process) { IsBackground = true };
            renderThread.Start();

            //

            void process()
            {
                // Firing begin event
                context.dispatcher.Invoke(() => RenderStart?.Invoke());
                // Render process
                lock (Paint)
                {
                    RenderRoutine(context);
                }
                // Firing end event
                context.dispatcher.Invoke(() => RenderComplete?.Invoke());
            }
        }

        public void Stop()
        {
            if (renderThread != null)
            {
                renderThread.Abort();
                renderThread = null;
            }
        }

        public virtual RenderContext BuildContext(Dispatcher dispatcher)
        {
            return new RenderContext
            {
                width = Paint.Width,
                height = Paint.Height,
                dispatcher = dispatcher,
            };
        }

        protected abstract void RenderRoutine(RenderContext context);

        protected void BatchScreen(RenderContext context, OnBatchScreen onBatch) 
        {
            int width = context.width;
            int height = context.height;
            Dispatcher dispatcher = context.dispatcher;
            //
            for (int y = 0; y < height; y += BatchSize)
            {
                for (int x = 0; x < width; x += BatchSize)
                {
                    int sizeX = Math.Min(BatchSize, width - x - 1);
                    int sizeY = Math.Min(BatchSize, height - y - 1);
                    //
                    Color[,] tile = onBatch(x, y, sizeX, sizeY);
                    //
                    dispatcher.Invoke(() => Paint.SetPixels(x, y, tile));

                    /*dispatcher.Invoke(() => {
                        for (int localY = 0; localY < sizeY; localY++)
                        {
                            int globalY = y + localY;

                            for (int localX = 0; localX < sizeX; localX++)
                            {
                                int globalX = x + localX;
                                //
                                Paint.SetPixel(globalX, globalY, (int)tile[localX, localY]);
                            }
                        }
                    });*/
                }
            }
        }

        protected delegate Color[,] OnBatchScreen(int ix, int iy, int sizeX, int sizeY);
    }

    public struct RenderContext
    {
        public int width;
        public int height;
        public Dispatcher dispatcher;
    }
}
