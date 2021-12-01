using RenderBox.Rendering;

namespace RenderBox.Options
{
    public interface IOptionsPage<T> where T : Renderer
    {
        void UseSource(T source);
    }
}
