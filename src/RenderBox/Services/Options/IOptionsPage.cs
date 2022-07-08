using RenderBox.Services.Rendering;

namespace RenderBox.Services.Options
{
    public interface IOptionsPage<T> where T : Renderer
    {
        void UseSource(T source);
    }
}
