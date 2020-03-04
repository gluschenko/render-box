using System;
using System.Windows.Threading;
using PathTracerSharp.Rendering;

namespace PathTracerSharp.Modules
{
    public class PerlinRenderer : Renderer
    {
        public PerlinRenderer(Paint paint) : base(paint)
        {

        }

        public override RenderContext BuildContext(Dispatcher dispatcher)
        {
            throw new NotImplementedException();
        }

        protected override void RenderRoutine(RenderContext context)
        {
            throw new NotImplementedException();
        }
    }
}
