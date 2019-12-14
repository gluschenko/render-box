using System;

namespace PathTracerSharp.Shapes
{
    public class Sphere : Shape
    {
        public float radius;

        public Sphere(Vector position, Color diffuse) : base(position, diffuse) { }

        public Sphere(Vector position, float radius, Color diffuse) : base(position, diffuse) 
        {
            this.radius = radius;
        }

        public override float GetIntersection(Ray ray, out Hit hit)
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

        public override Vector CalcNormal(Vector pos) 
        {
            return Vector.Normalize(pos - position);
        }
    }
}
