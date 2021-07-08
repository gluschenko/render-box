using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public struct Ray
    {
        public Vector3 Origin { get; set; }
        public Vector3 Direction { get; set; }

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }

    public struct Hit
    {
        public Vector3 Position { get; set; }
        public Shape HitObject { get; set; }

        public bool IsHitting => HitObject != null;
    }
}
