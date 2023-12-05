using RenderBox.Shared.Core;

namespace RenderBox.Shared.Modules.Mandelbrot.Filters
{
    public abstract class IPaletteFilter
    {
        public abstract Color GetColor(double rate, double zoom);
    }
}
