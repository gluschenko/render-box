using RenderBox.Core;
using System.Collections.Generic;

namespace RenderBox.Shared.Modules.PathTracer.Shapes
{
    public class Mesh : Shape
    {
        public List<Vector3> Vertices { get; private set; }
        public List<int> Indices { get; private set; }
        public int TrianglesCount { get; private set; }

        public Mesh(Vector3 position, Color diffuse) : base(position, diffuse)
        {
            Vertices = new List<Vector3>();
            Indices = new List<int>();
        }

        public override double GetIntersection(Ray ray, out Hit hit)
        {
            bool intersect = false;

            hit = new Hit();

            for (int k = 0; k < TrianglesCount; ++k)
            {
                var v0 = Vertices[Indices[k * 3]];
                var v1 = Vertices[Indices[k * 3 + 1]];
                var v2 = Vertices[Indices[k * 3 + 2]];
                double t = 0, u = 0, v = 0;
                if (rayTriangleIntersect(v0, v1, v2, ray, ref t, ref u, ref v))
                {
                    //uv.x = u;
                    //uv.y = v;
                    //index = k;
                    intersect |= true;
                }
            }

            return intersect ? 1.0 : 0.0;
        }

        public override Vector3 CalcNormal(Vector3 pos)
        {
            return Vector3.Zero;
        }

        public void SetData(List<Vector3> verts, List<int> indices)
        {
            Vertices = verts;
            Indices = indices;
            TrianglesCount = indices.Count / 3;
        }

        private bool rayTriangleIntersect(Vector3 v0, Vector3 v1, Vector3 v2, Ray ray, ref double near, ref double u, ref double v)
        {
            var edge1 = v1 - v0;
            var edge2 = v2 - v0;
            var pvec = Vector3.Cross(ray.direction, edge2);

            double det = Vector3.Dot(edge1, pvec);
            if (det == 0 || det < 0) return false;

            var tvec = ray.origin - v0;
            u = Vector3.Dot(tvec, pvec);
            if (u < 0 || u > det) return false;

            var qvec = Vector3.Cross(tvec, edge1);
            v = Vector3.Dot(ray.direction, qvec);
            if (v < 0 || u + v > det) return false;

            double invDet = 1 / det;

            near = Vector3.Dot(edge2, qvec) * invDet;
            u *= invDet;
            v *= invDet;

            return true;
        }
    }
}
