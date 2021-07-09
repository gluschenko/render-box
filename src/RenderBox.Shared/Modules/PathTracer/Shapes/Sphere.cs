using RenderBox.Core;
using System;
using static RenderBox.Core.VectorMath;

namespace RenderBox.Shared.Modules.PathTracer.Shapes
{
    public class Sphere : Shape
    {
        public float Radius { get; set; }

        public Sphere(Vector3 position, float radius, Color diffuse) : base(position, diffuse)
        {
            Radius = radius;
        }

        public override bool GetIntersection(Ray ray, double maxDistance, out Hit hit, out double distance)
        {
            hit = new Hit();

            var delta = ray.Origin - Position;

            var a = Dot(ray.Direction, ray.Direction);
            var b = 2 * Dot(ray.Direction, delta);
            var c = Dot(delta, delta) - Radius * Radius;

            double dt = b * b - 4 * a * c;

            if (dt < 0)
            {
                distance = 0;
                return false;
            }
            else
            {
                double D = (-b - Math.Sqrt(dt)) / (a * 2);
                if (D < 0)
                {
                    distance = 0;
                    return false;
                }

                hit.Position = ray.Origin + ray.Direction * (float)D;
                hit.Normal = CalcNormal(hit.Position);
                hit.HitObject = this;

                var dist = Distance(hit.Position, ray.Origin);
                distance = dist;

                return dist < maxDistance;
            }
        }

        public override Vector3 CalcNormal(Vector3 pos)
        {
            return Normalize(pos - Position);
        }
    }
}
