using System;
using PathTracerSharp.Rendering;

namespace PathTracerSharp.Shapes
{
    public class Mesh : Shape
    {
        public Vector Vertices { get; private set; }
        public int VertexIndex { get; private set; }
        public int TrianglesCount { get; private set; }

        //public Mesh(Vector position, Color diffuse) : base(position, diffuse) { }

        public Mesh(Vector position, Color diffuse, int o) : base(position, diffuse)
        {

        }

        public override double GetIntersection(Ray ray, out Hit hit)
        {
            throw new NotImplementedException();
        }

        public override Vector CalcNormal(Vector pos)
        {
            throw new NotImplementedException();
        }
    }
}
