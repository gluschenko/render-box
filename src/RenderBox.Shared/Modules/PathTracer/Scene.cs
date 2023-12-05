using RenderBox.Shared.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public class Scene
    {
        public Color BackgroundColor { get; set; } = new Color(.2f, .2f, .2f);
        public Color AmbientColor { get; set; } = new Color(.1f, .1f, .1f);
        public List<Shape> Shapes { get; set; } = new List<Shape>();
        public IEnumerable<Light> Lights { get; private set; }

        public bool LightingEnabled { get; set; } = true;
        public bool ShadowsEnabled { get; set; } = true;
        public bool SoftShadows { get; set; } = true;
        public bool AmbientOcclusion { get; set; } = true;

        public int GISamples { get; set; } = 16;

        public Scene()
        {
        }

        public void UpdateLights()
        {
            Lights = Shapes.Where(x => x.Light != null).Select(x => x.Light).ToArray();
        }
    }
}
