using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
