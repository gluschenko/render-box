using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracerSharp.Rendering
{
    public class Camera
    {
        public Vector Position { get; set; }
        public Vector Rotation { get; set; }
        public int MaxDepth { get; set; } = 3;
        public double FOV { get; set; } = 0.5;

        public Camera(Vector position, Vector rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
