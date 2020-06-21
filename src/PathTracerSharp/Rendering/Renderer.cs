using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using PathTracerSharp.Core;

namespace PathTracerSharp.Rendering
{
    public delegate void RenderStartHandler();
    public delegate void RenderCompleteHandler();

    public abstract class Renderer : IDisposable
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
                context.dispatcher.Invoke(() => RenderStart?.Invoke());

                lock (Paint)
                {
                    RenderRoutine(context);
                }

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

        public void Dispose()
        {
            Stop();
            Paint?.Dispose();
        }

        public virtual void OnKeyPress(Key key, Action onRender)
        {
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
