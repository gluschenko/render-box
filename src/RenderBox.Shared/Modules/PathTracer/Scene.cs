using RenderBox.Core;
using System.Collections.Generic;

namespace RenderBox.Shared.Modules.PathTracer
{
    public class Scene
    {
        public Color BackgroundColor { get; set; }
        public Color AmbientColor { get; set; }
        public List<Shape> Shapes { get; set; }
        public List<Light> Lights { get; set; }

        public int Samples { get; set; }

        public Scene()
        {
            BackgroundColor = new Color(.2f, .2f, .2f);
            AmbientColor = new Color(.1f, .1f, .1f);
            Shapes = new List<Shape>();
            Lights = new List<Light>();
        }
    }
}
