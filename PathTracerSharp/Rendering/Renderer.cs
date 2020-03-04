using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using PathTracerSharp.Core;
using PathTracerSharp.Modules.PathTracer;

namespace PathTracerSharp.Rendering
{
    public delegate void RenderStartHandler();
    public delegate void RenderCompleteHandler();

    public abstract class Renderer : Renderer<RenderContext>
    {
        public Renderer(Paint paint) : base(paint) { }
    }

    public abstract class Renderer<T> where T : RenderContext
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

        public void Render(T context)
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

        public abstract T BuildContext(Dispatcher dispatcher);
        protected abstract void RenderRoutine(T context);
    }

    public class RenderContext
    {
        public int width;
        public int height;
        public Dispatcher dispatcher;
    }
}
