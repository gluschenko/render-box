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

        public override double GetIntersection(Ray ray, out Hit hit)
        {
            hit = new Hit();

            var delta = ray.Origin - Position;

            var a = Dot(ray.Direction, ray.Direction);
            var b = 2 * Dot(ray.Direction, delta);
            var c = Dot(delta, delta) - Radius * Radius;

            double dt = b * b - 4 * a * c;

            if (dt < 0)
            {
                return double.NaN;
            }
            else
            {
                double D = (-b - Math.Sqrt(dt)) / (a * 2);
                if (D < 0)
                {
                    return double.NaN;
                }

                hit.Position = ray.Origin + ray.Direction * (float)D;
                hit.HitObject = this;

                return Distance(hit.Position, ray.Origin);
            }
        }

        public override Vector3 CalcNormal(Vector3 pos)
        {
            return Normalize(pos - Position);
        }
    }
}
