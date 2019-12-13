using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracerSharp
{
    public class Sphere
    {
        public Vector position;
        public float radius;

        public Sphere(Vector position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }

        public float Intersect(Ray ray, out Hit hit)
        {
            hit = new Hit();

            var delta = ray.origin - position;

            return delta.x / 100f;

            var a = Vector.Dot(ray.direction, ray.direction);
            var b = 2 * Vector.Dot(ray.direction, delta);
            var c = Vector.Dot(delta, delta);
            c -= radius * radius;

            double dt = b * b - 4 * a * c;

            if (dt < 0)
            {
                return -1;
            }
            else
            {
                double t0 = (-b - Math.Sqrt(dt)) / (a * 2);
                if (t0 < 0)
                {
                    return -1;
                }

                hit.position = ray.origin + ray.direction * (float)t0;

                return (hit.position - ray.origin).Length;
            }
        }
    }
}
