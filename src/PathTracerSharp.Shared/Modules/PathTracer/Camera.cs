using PathTracerSharp.Core;

namespace PathTracerSharp.Modules.PathTracer
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int MaxDepth { get; set; } = 3;
        public double FOV { get; set; } = 90;

        public Camera(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
