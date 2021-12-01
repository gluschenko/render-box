using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using RenderBox.Core;
using RenderBox.Options;

namespace RenderBox.Rendering
{
    public delegate void RenderStartHandler();
    public delegate void RenderCompleteHandler();

    public abstract class Renderer : IDisposable
    {
        public int BatchSize { get; set; } = 32;
        public Paint Paint { get; private set; }

        public event RenderStartHandler OnRenderStarted;
        public event RenderCompleteHandler OnRenderComplete;


        private Thread _renderThread;

        public Renderer(Paint paint)
        {
            Paint = paint;
        }

        public Type GetOptionPageType()
        {
            var type = GetType();
            var optionsType = typeof(IOptionsPage<>);

            var optionsPages = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface)
                .Where(x => x.GetInterfaces().Any(x => x.Name == optionsType.Name && x.GetGenericArguments().Contains(type)))
                .ToArray();

            return optionsPages.FirstOrDefault();
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
                    context.Dispatcher.Invoke(() => OnRenderStarted?.Invoke());

                    lock (Paint)
                    {
                        RenderScreen(context);
                    }

                    context.Dispatcher.Invoke(() => OnRenderComplete?.Invoke());
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

        public void Reset(Paint paint)
        {
            Stop();
            Paint = paint;
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
            using var threadManager = new ThreadManager();

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

                    void DrawTile()
                    {
                        var tile = renderScreenBatch(ix, iy, sizeX, sizeY);
                        dispatcher.Invoke(() => Paint.SetPixels(ix, iy, tile));
                    }

                    threadManager.Push(DrawTile, priority);
                }
            }

            using var semaphore = new SemaphoreSlim(1, 1);

            try
            {
                semaphore.Wait();
                threadManager.Start(Environment.ProcessorCount, () => semaphore.Release());
                semaphore.Wait();
            }
            finally
            {
                semaphore.Release();
            }

            /*
            using var locker = new EventWaitHandle(false, EventResetMode.AutoReset);
            threadManager.Start(Environment.ProcessorCount, () => locker.Set());
            WaitHandle.WaitAll(new[] { locker });
            locker.Reset();
            */
        }

        protected delegate Color[,] RenderScreenBatch(int ix, int iy, int sizeX, int sizeY);
        protected delegate int GetRenderPriority(int ix, int iy);

    }

    public struct RenderContext
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double Scale { get; set; }
        public Dispatcher Dispatcher { get; set; }
    }
}
