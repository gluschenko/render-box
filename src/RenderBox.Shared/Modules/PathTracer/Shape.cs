using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public abstract class Shape : IShape
    {
        public Vector3 Position { get; set; }
        public Material Material { get; set; }

        public Shape(Vector3 pos, Color diffuse)
        {
            Position = pos;
            //
            Material = new Material
            {
                diffuse = diffuse
            };
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
