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

            for (int k = 0; k < TrianglesCount; ++k)
            {
                var v0 = Position + Vertices[Indices[k * 3 + 0]];
                var v1 = Position + Vertices[Indices[k * 3 + 1]];
                var v2 = Position + Vertices[Indices[k * 3 + 2]];

                if (RayTriangleIntersect(v0, v1, v2, ray, out var localHit))
                {
                    if (localHit.IsHitting)
                    {
                        isMatch = true;
                        hit = localHit;
                    }
                }
            }

            distance = (hit.Position - ray.Origin).Length;

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

        private bool RayTriangleIntersect(Vector3 v0, Vector3 v1, Vector3 v2, Ray ray, out Hit hit)
        {
            hit = new Hit();

            var e1 = v1 - v0;
            var e2 = v2 - v0;
            var p = Cross(ray.Direction, e2);

            var det = Dot(e1, p);
            if (det == 0 || det < 0)
            {
                return false; // parallel to the plane
            }

            var f = 1.0f / det;
            var s = ray.Origin - v0;
            var u = f * Dot(s, p);

            if (u < 0.0f || u > 1.0f)
            {
                return false; // but outside the triangle
            }

            var q = Cross(s, e1);
            var v = f * Dot(ray.Direction, q);

            if (v < 0.0f || (u + v) > 1.0f)
            {
                return false; // but outside the triangle
            }

            var t = f * Dot(e2, q);

            if(t <= 0)
            {
                return false;
            }

            hit.HitObject = this;
            hit.Position = ray.PointAt(t);
            hit.Normal = Normalize(Cross(e1, e2));
            return true;
        }
    }
}
