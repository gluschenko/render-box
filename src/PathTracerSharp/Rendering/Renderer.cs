using PathTracerSharp.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

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
        private Thread _renderThread;
        //
        public Renderer(Paint paint)
        {
            Paint = paint;
        }

        public void Render(Dispatcher dispatcher) => Render(BuildContext(dispatcher));

        private void Render(RenderContext context)
        {
            Stop();

            _renderThread = new Thread(Proc) { IsBackground = true };
            _renderThread.Start();

            //

            void Proc()
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
            if (_renderThread != null)
            {
                _renderThread.Interrupt();
                _renderThread = null;
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
            using var _threadManager = new ThreadManager();

            var width = context.width;
            var height = context.height;
            var dispatcher = context.dispatcher;
            //
            for (var y = 0; y < height; y += BatchSize)
            {
                for (var x = 0; x < width; x += BatchSize)
                {
                    var localX = x;
                    var localY = y;
                    var sizeX = Math.Min(BatchSize, width - localX - 1);
                    var sizeY = Math.Min(BatchSize, height - localY - 1);
                    //
                    _threadManager.Push(() =>
                    {
                        var tile = onBatch(localX, localY, sizeX, sizeY);
                        dispatcher.Invoke(() => Paint.SetPixels(localX, localY, tile));
                    });
                }
            }

            var locker = new EventWaitHandle(false, EventResetMode.AutoReset);
            _threadManager.Start(Environment.ProcessorCount, () => locker.Set());
            WaitHandle.WaitAll(new[] { locker });
            locker.Reset();
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
