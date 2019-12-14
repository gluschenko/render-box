using System;

namespace PathTracerSharp
{
    public struct Ray
    {
        public Vector origin;
        public Vector direction;

        public Ray(Vector origin, Vector direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }

    public struct Hit
    {
        public Vector position;
        public Shape hitObject;

        public bool IsHitting => hitObject != null;
    }
}
