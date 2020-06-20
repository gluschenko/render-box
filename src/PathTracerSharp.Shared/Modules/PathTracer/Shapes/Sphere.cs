using System;
using PathTracerSharp.Core;

namespace PathTracerSharp.Shared.Modules.PathTracer.Shapes
{
    public class Sphere : Shape
    {
        public float radius;

        //public Sphere(Vector position, Color diffuse) : base(position, diffuse) { }

        public Sphere(Vector3 position, float radius, Color diffuse) : base(position, diffuse) 
        {
            this.radius = radius;
        }

        public override double GetIntersection(Ray ray, out Hit hit)
        {
            hit = new Hit();

            var delta = ray.origin - position;

            var a = Vector3.Dot(ray.direction, ray.direction);
            var b = 2 * Vector3.Dot(ray.direction, delta);
            var c = Vector3.Dot(delta, delta) - radius * radius;

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

                return Vector3.Distance(hit.position, ray.origin);
            }
        }

        public override Vector3 CalcNormal(Vector3 pos) 
        {
            return Vector3.Normalize(pos - position);
        }
    }
}
