using PathTracerSharp.Core;

namespace PathTracerSharp.Modules.PathTracer
{
    public abstract class Shape : IShape
    {
        public Vector3 position;
        public Material material;

        public Shape(Vector3 pos, Color diffuse)
        {
            position = pos;
            //
            material = new Material();
            material.diffuse = diffuse;
        }

        public abstract Vector3 CalcNormal(Vector3 pos);
        public abstract double GetIntersection(Ray ray, out Hit hit);
    }

    public interface IShape
    {
        double GetIntersection(Ray ray, out Hit hit);
        Vector3 CalcNormal(Vector3 pos);
    }
}
