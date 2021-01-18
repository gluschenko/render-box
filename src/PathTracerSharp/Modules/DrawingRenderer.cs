﻿using PathTracerSharp.Core;
using PathTracerSharp.Rendering;

namespace PathTracerSharp.Modules
{
    public class DrawingRenderer : Renderer
    {
        public DrawingRenderer(Paint paint) : base(paint)
        {

        }

        protected override void RenderScreen(RenderContext context)
        {
            context.dispatcher.Invoke(() => 
            {
                Paint.FillRect(10, 10, 80, 40, Color.Green);
                Paint.FillRect(10, 60, 80, 40, Color.Yellow);
                Paint.FillRect(10, 120, 80, 40, Color.Red);
                Paint.FillRect(10, 180, 80, 40, Color.Blue);
            });
        }
    }
}
