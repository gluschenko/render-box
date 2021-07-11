using RenderBox.Core;
using System.Collections.Generic;
using System.Linq;

namespace RenderBox.Shared.Modules.PathTracer
{
    public class Scene
    {
        public Color BackgroundColor { get; set; }
        public Color AmbientColor { get; set; }
        public List<Shape> Shapes { get; set; }
        public IEnumerable<Light> Lights { get; set; }

        public int Samples { get; set; }

        public Scene()
        {
            BackgroundColor = new Color(.2f, .2f, .2f);
            AmbientColor = new Color(.1f, .1f, .1f);
            Shapes = new List<Shape>();
        }

        public void UpdateLights()
        {
            Lights = Shapes.Where(x => x.Light != null).Select(x => x.Light).ToArray();
        }
    }
}
