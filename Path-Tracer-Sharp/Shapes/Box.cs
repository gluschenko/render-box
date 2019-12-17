using System;

namespace PathTracerSharp.Shapes
{
    public class Box : Shape
    {
        public Vector pointA, pointB;

        public Box(Vector position, Color diffuse) : base(position, diffuse) { }
        public Box(Vector position, Vector pointA, Vector pointB, Color diffuse) : base(position, diffuse) 
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }

        public override float GetIntersection(Ray ray, out Hit hit)
        {
            hit = new Hit();

            float tmin = (pointA.x - ray.origin.x) / ray.direction.x;
            float tmax = (pointB.x - ray.origin.x) / ray.direction.x;

            if (tmin > tmax) 
            {
                (tmin, tmax) = (tmax, tmin);
            }

            float tymin = (pointA.y - ray.origin.y) / ray.direction.y;
            float tymax = (pointB.y - ray.origin.y) / ray.direction.y;

            if (tymin > tymax)
            {
                (tymin, tymax) = (tymax, tymin);
            }

            if ((tmin > tymax) || (tymin > tmax))
                return -1;

            if (tymin > tmin)
                tmin = tymin;

            if (tymax < tmax)
                tmax = tymax;

            float tzmin = (pointB.z - ray.origin.z) / ray.direction.z;
            float tzmax = (pointB.z - ray.origin.z) / ray.direction.z;

            if (tzmin > tzmax)
            {
                (tzmin, tzmax) = (tzmax, tzmin);
            }

            if ((tmin > tzmax) || (tzmin > tmax))
                return -1;

            /*if (tzmin > tmin)
                tmin = tzmin;

            if (tzmax < tmax)
                tmax = tzmax;*/

            return 1;
        }

        public override Vector CalcNormal(Vector pos)
        {
            return Vector.Normalize(pos - position);
        }
    }
}
