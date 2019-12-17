using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracerSharp.Shapes
{
    public class Mesh : Shape
    {
        public Mesh(Vector position, Color diffuse) : base(position, diffuse) { }

        public override float GetIntersection(Ray ray, out Hit hit)
        {
            throw new NotImplementedException();
        }

        public override Vector CalcNormal(Vector pos)
        {
            throw new NotImplementedException();
        }
    }
}
