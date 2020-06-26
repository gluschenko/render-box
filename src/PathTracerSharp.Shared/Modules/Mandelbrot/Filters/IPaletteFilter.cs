using PathTracerSharp.Core;

namespace PathTracerSharp.Shared.Modules.Mandelbrot.Filters
{
    public abstract class IPaletteFilter
    {
        public abstract Color GetColor(double rate, double zoom);
    }
}
