using System;
using PathTracerSharp.Core;

namespace PathTracerSharp.Modules.PathTracer.Shapes
{
    public class Mesh : Shape
    {
        public Vector3 Vertices { get; private set; }
        public int VertexIndex { get; private set; }
        public int TrianglesCount { get; private set; }

        //public Mesh(Vector position, Color diffuse) : base(position, diffuse) { }

        public Mesh(Vector3 position, Color diffuse, int o) : base(position, diffuse)
        {

        }

        public override double GetIntersection(Ray ray, out Hit hit)
        {
            throw new NotImplementedException();
        }

        public override Vector3 CalcNormal(Vector3 pos)
        {
            throw new NotImplementedException();
        }
    }
}
