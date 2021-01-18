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
        private Thread[] threadPool;
        //
        public Renderer(Paint paint)
        {
            Paint = paint;
        }

        public void Render(Dispatcher dispatcher) => Render(BuildContext(dispatcher));

        private void Render(RenderContext context)
        {
            Stop();

            renderThread = new Thread(process) { IsBackground = true };
            renderThread.Start();

            //

            void process()
            {
                try
                {
                    context.dispatcher.Invoke(() => RenderStart?.Invoke());

                    lock (Paint)
                    {
                        RenderScreen(context);
                    }

                    context.dispatcher.Invoke(() => RenderComplete?.Invoke());
                }
                catch (ThreadInterruptedException) 
                {
                }
            }
        }

        public void Stop()
        {
            if (renderThread != null)
            {
                //renderThread.Abort();
                renderThread.Interrupt();
                renderThread = null;
            }
        }

        public void Dispose()
        {
            Stop();
            Paint?.Dispose();
            GC.SuppressFinalize(this);
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

        protected abstract void RenderScreen(RenderContext context);

        protected void BatchScreen(RenderContext context, OnBatchScreen onBatch) 
        {
            var width = context.width;
            var height = context.height;
            var dispatcher = context.dispatcher;
            //
            for (var y = 0; y < height; y += BatchSize)
            {
                for (var x = 0; x < width; x += BatchSize)
                {
                    var sizeX = Math.Min(BatchSize, width - x - 1);
                    var sizeY = Math.Min(BatchSize, height - y - 1);
                    //
                    var tile = onBatch(x, y, sizeX, sizeY);
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
