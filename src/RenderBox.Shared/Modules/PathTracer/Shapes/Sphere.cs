using RenderBox.Core;
using System;
using static RenderBox.Core.VectorMath;

namespace RenderBox.Shared.Modules.PathTracer.Shapes
{
    public class Sphere : Shape
    {
        public float Radius { get; set; }

        //public Sphere(Vector position, Color diffuse) : base(position, diffuse) { }

        public Sphere(Vector3 position, float radius, Color diffuse) : base(position, diffuse)
        {
            Radius = radius;
        }

        public override double GetIntersection(Ray ray, out Hit hit)
        {
            hit = new Hit();

            var delta = ray.origin - Position;

            var a = Dot(ray.direction, ray.direction);
            var b = 2 * Dot(ray.direction, delta);
            var c = Dot(delta, delta) - Radius * Radius;

            double dt = b * b - 4 * a * c;

            if (dt < 0)
            {
                return -1;
            }
            else
            {
                double D = (-b - Math.Sqrt(dt)) / (a * 2);
                if (D < 0)
                {
                    return -1;
                }

                hit.position = ray.origin + ray.direction * (float)D;
                hit.hitObject = this;

                return Distance(hit.position, ray.origin);
            }
        }

        public override Vector3 CalcNormal(Vector3 pos)
        {
            return Normalize(pos - Position);
        }
    }
}
