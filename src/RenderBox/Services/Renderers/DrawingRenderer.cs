using System;
using RenderBox.Core;
using RenderBox.Services.Rendering;

namespace RenderBox.Services.Renderers
{
    public class DrawingRenderer : Renderer
    {
        private Action<RenderContext> _render;

        public DrawingRenderer(Paint paint) : base(paint)
        {
            _render = (context) =>
            {
                Paint.FillRect(10, 10, 80, 40, Color.Green);
                Paint.FillRect(10, 60, 80, 40, Color.Yellow);
                Paint.FillRect(10, 120, 80, 40, Color.Red);
                Paint.FillRect(10, 180, 80, 40, Color.Blue);
            };
        }

        public void SetRender(Action<RenderContext> render)
        {
            _render = render;
        }

        protected override void RenderScreen(RenderContext context)
        {
            context.Dispatcher.Invoke(() =>
            {
                _render(context);
            });
        }
    }
}
