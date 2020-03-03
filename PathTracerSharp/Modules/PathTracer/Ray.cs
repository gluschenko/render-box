using System;
using PathTracerSharp.Core;

namespace PathTracerSharp.Modules.PathTracer
{
    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }

    public struct Hit
    {
        public Vector3 position;
        public Shape hitObject;

        public bool IsHitting => hitObject != null;
    }
}
