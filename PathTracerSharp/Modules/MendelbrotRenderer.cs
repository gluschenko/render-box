using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using PathTracerSharp.Rendering;

namespace PathTracerSharp.Modules
{
    public class MendelbrotRenderer : Renderer
    {
        public MendelbrotRenderer(Paint paint) : base(paint)
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
