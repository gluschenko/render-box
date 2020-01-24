
namespace PathTracerSharp.Rendering
{
    public abstract class Shape : IShape
    {
        public Vector position;
        public Material material;

        public Shape(Vector pos, Color diffuse)
        {
            position = pos;
            //
            material = new Material();
            material.diffuse = diffuse;
        }

        public abstract Vector CalcNormal(Vector pos);
        public abstract double GetIntersection(Ray ray, out Hit hit);
    }

    public interface IShape
    {
        double GetIntersection(Ray ray, out Hit hit);
        Vector CalcNormal(Vector pos);
    }
}
