using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public enum ShadingType
    {
        Diffuse = 0,
        DiffuseGlossy = 1,
        Reflection = 2,
        Refraction = 3,
    }

    public class Material
    {
        public ShadingType ShadingType { get; set; }
        public Color Color { get; set; }
        public Color Specular { get; set; }

        public float Reflection { get; set; }
        public float Refraction { get; set; }
        public float RefractionEta { get; set; }

        public bool IsMetallic { get; set; }

        public Material()
        {
            Color = Color.White;
            Specular = Color.White;
            ShadingType = ShadingType.Diffuse;

            Reflection = 0;
            Refraction = 0;
            RefractionEta = -0.5f;

            IsMetallic = false;
        }
    }
}
