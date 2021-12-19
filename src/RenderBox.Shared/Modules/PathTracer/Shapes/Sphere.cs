using RenderBox.Core;
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
                var D = (-b - MathHelpres.FastSqrt(dt)) / (a * 2);
                if (D < 0)
                {
                    distance = 0;
                    return false;
                }

                var position = ray.Origin + ray.Direction * (float)D;
                var dist = Distance(position, ray.Origin);

                if (dist > maxDistance)
                {
                    distance = dist;
                    return false;
                }

                hit.Position = position;
                hit.Normal = CalcNormal(hit.Position);
                hit.HitObject = this;
                distance = dist;

                return hit.IsHitting;
            }
        }

        public override Vector3 CalcNormal(Vector3 pos)
        {
            return Normalize(pos - Position);
        }

        public override Vector3 GetLightEmission(Vector3 random)
        {
            return Normalize(random) * Radius;
        }
    }
}
