using RenderBox.Shared.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public interface IShape
    {
        Vector3 Position { get; set; }
        Material Material { get; set; }
        Vector3 CalcNormal(Vector3 pos);
        bool GetIntersection(Ray ray, double maxDistance, out Hit hit, out double distance);
        Vector3 GetLightEmission(Vector3 random);
    }

    public abstract class Shape : IShape
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Material Material { get; set; }
        public Light? Light { get; private set; }

        public Shape(Vector3 pos, Color diffuse)
        {
            Position = pos;
            Rotation = Vector3.Zero;
            //
            Material = new Material
            {
                Color = diffuse
            };
        }

        public Shape SetLight(Light light)
        {
            light.Shape = this;
            light.Color = Material.Color;
            Light = light;
            return this;
        }

        public Shape SetRotation(Vector3 rotation)
        {
            Rotation = rotation;
            return this;
        }

        public Shape SetRotationDegrees(Vector3 rotation)
        {
            Rotation = new Vector3(
                MathHelpres.DegToRad(rotation.x),
                MathHelpres.DegToRad(rotation.y),
                MathHelpres.DegToRad(rotation.z));

            return this;
        }

        protected Matrix4x4 RotationMatrix => Matrix4x4.CreateRotation(Rotation);
        protected Matrix4x4 InverseRotationMatrix => RotationMatrix.Transpose();

        protected Vector3 LocalToWorldPoint(Vector3 point)
        {
            return Position + RotationMatrix.TransformPoint(point);
        }

        protected Vector3 WorldToLocalPoint(Vector3 point)
        {
            return InverseRotationMatrix.TransformPoint(point - Position);
        }

        protected Vector3 LocalToWorldDirection(Vector3 direction)
        {
            return RotationMatrix.TransformDirection(direction);
        }

        protected Vector3 WorldToLocalDirection(Vector3 direction)
        {
            return InverseRotationMatrix.TransformDirection(direction);
        }

        public abstract Vector3 CalcNormal(Vector3 pos);
        public abstract bool GetIntersection(Ray ray, double maxDistance, out Hit hit, out double distance);

        public virtual Vector3 GetLightEmission(Vector3 random)
        {
            return Vector3.Zero;
        }
    }
}
