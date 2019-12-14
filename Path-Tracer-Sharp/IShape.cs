
namespace PathTracerSharp
{
    public abstract class Shape : IShape
    {
        public Vector position;
        public Color ambient, diffuse, specular;

        public Shape(Vector position, Color diffuse)
        {
            this.position = position;
            this.diffuse = diffuse;
        }

        public abstract Vector CalcNormal(Vector pos);
        public abstract float GetIntersection(Ray ray, out Hit hit);
    }

    public interface IShape
    {
        float GetIntersection(Ray ray, out Hit hit);
        Vector CalcNormal(Vector pos);
    }
}
