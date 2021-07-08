using RenderBox.Core;
using System.Collections.Generic;
using static RenderBox.Core.VectorMath;

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

        public override bool GetIntersection(Ray ray, double maxDistance, out Hit hit, out double distance)
        {
            bool isMatch = false;

            hit = new Hit();
            var hitPosition = new Vector3();

            for (int k = 0; k < TrianglesCount; ++k)
            {
                var v0 = Position + Vertices[Indices[k * 3 + 0]];
                var v1 = Position + Vertices[Indices[k * 3 + 1]];
                var v2 = Position + Vertices[Indices[k * 3 + 2]];
                double t = 0, u = 0, v = 0;

                if (RayTriangleIntersect(v0, v1, v2, ray, ref t, ref u, ref v))
                {
                    //uv.x = u;
                    //uv.y = v;
                    //index = k;
                    hitPosition = (v0 + v1 + v2) / 3;
                    isMatch |= true;
                }
            }

            distance = 0;

            if (isMatch)
            {
                hit.HitObject = this;
                hit.Position = hitPosition;
            }

            return isMatch;
        }

        public override Vector3 CalcNormal(Vector3 pos)
        {
            return Normalize(pos - Position);
        }

        public void SetData(List<Vector3> verts, List<int> indices)
        {
            Vertices = verts;
            Indices = indices;
            TrianglesCount = indices.Count / 3;
        }

        private bool RayTriangleIntersect(Vector3 v0, Vector3 v1, Vector3 v2, Ray ray, ref double near, ref double u, ref double v)
        {
            var edge1 = v1 - v0;
            var edge2 = v2 - v0;
            var pvec = Cross(ray.Direction, edge2);

            double det = Dot(edge1, pvec);
            if (det == 0 || det < 0)
            {
                return false;
            }

            var tvec = ray.Origin - v0;
            u = Dot(tvec, pvec);
            if (u < 0 || u > det)
            {
                return false;
            }

            var qvec = Cross(tvec, edge1);
            v = Dot(ray.Direction, qvec);
            if (v < 0 || u + v > det)
            {
                return false;
            }

            double invDet = 1 / det;

            near = Dot(edge2, qvec) * invDet;
            u *= invDet;
            v *= invDet;

            return true;
        }
    }
}
