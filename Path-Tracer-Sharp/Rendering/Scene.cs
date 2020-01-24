using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracerSharp.Rendering
{
    public class Scene
    {
        public Color BackgroundColor { get; set; }
        public List<Shape> Shapes { get; set; }
        public List<Light> Lights { get; set; }

        public Scene()
        {
            BackgroundColor = new Color(.2f, .2f, .2f);
            Shapes = new List<Shape>();
            Lights = new List<Light>();
        }
    }
}
