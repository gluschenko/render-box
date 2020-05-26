using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PathTracerSharp.Web.Pages
{
    public class IndexComponent : ComponentBase
    {
        private Canvas2DContext _context;

        protected BECanvasComponent _canvasReference;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            _context = await _canvasReference.CreateCanvas2DAsync();
            await _context.SetFillStyleAsync("green");

            await _context.FillRectAsync(10, 100, 100, 100);

            await _context.SetFontAsync("48px Arial");
            await _context.FillTextAsync("Hello Blazor!!!", 10, 100);
        }
    }
}
