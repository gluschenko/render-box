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
        public Color ambient, diffuse, specular;

        public Sphere(Vector position, float radius, Color diffuse)
        {
            this.position = position;
            this.radius = radius;
            this.diffuse = diffuse;
        }

        public float Intersect(Ray ray, out Hit hit)
        {
            hit = new Hit();

            var delta = ray.origin - position;

            var a = Vector.Dot(ray.direction, ray.direction);
            var b = 2 * Vector.Dot(ray.direction, delta);
            var c = Vector.Dot(delta, delta) - radius * radius;

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

                return Vector.Distance(hit.position, ray.origin);
            }
        }

        public static Hit FindClosest(List<Sphere> shapes, Ray ray) 
        {
            var closest = new Hit();

            float min_dist = float.MaxValue;

            foreach (var shape in shapes) 
            {
                float distance = shape.Intersect(ray, out Hit localHit);
                if (distance != -1 && distance < min_dist)
                {
                    min_dist = distance;
                    closest = localHit;
                }
            }
            return closest;
        }

        public Vector CalcNormal(Vector pos) 
        {
            return Vector.Normalize(pos - position);
        }
    }
}
