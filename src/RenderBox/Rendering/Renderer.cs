using RenderBox.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RenderBox.Rendering
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
                    context.Dispatcher.Invoke(() => RenderStart?.Invoke());

                    lock (Paint)
                    {
                        RenderScreen(context);
                    }

                    context.Dispatcher.Invoke(() => RenderComplete?.Invoke());
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
                Width = Paint.Width,
                Height = Paint.Height,
                Scale = Paint.Scale,
                Dispatcher = dispatcher,
            };
        }

        protected abstract void RenderScreen(RenderContext context);

        protected virtual void BatchScreen(RenderContext context, 
                                           RenderScreenBatch renderScreenBatch, 
                                           GetRenderPriority getRenderPriority = null) 
        {
            using var _threadManager = new ThreadManager();

            var width = context.Width;
            var height = context.Height;
            var dispatcher = context.Dispatcher;
            //
            for (var y = 0; y < height; y += BatchSize)
            {
                for (var x = 0; x < width; x += BatchSize)
                {
                    // to prevent on-stack closure
                    var ix = x;
                    var iy = y;
                    var sizeX = Math.Min(BatchSize, width - ix - 1);
                    var sizeY = Math.Min(BatchSize, height - iy - 1);
                    //
                    var priority = getRenderPriority?.Invoke(ix, iy) ?? 0;

                    _threadManager.Push(() =>
                    {
                        var tile = renderScreenBatch(ix, iy, sizeX, sizeY);
                        dispatcher.Invoke(() => Paint.SetPixels(ix, iy, tile));
                    }, priority);
                }
            }

            var locker = new EventWaitHandle(false, EventResetMode.AutoReset);
            _threadManager.Start(Environment.ProcessorCount, () => locker.Set());
            WaitHandle.WaitAll(new[] { locker });
            locker.Reset();
        }

        protected delegate Color[,] RenderScreenBatch(int ix, int iy, int sizeX, int sizeY);
        protected delegate int GetRenderPriority(int ix, int iy);
        
    }

    public struct RenderContext
    {
        public int Width;
        public int Height;
        public double Scale;
        public Dispatcher Dispatcher;
    }
}
