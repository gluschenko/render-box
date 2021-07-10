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
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }

        public float Reflection { get; set; }
        public float Refraction { get; set; }
        public float RefractionEta { get; set; }

        public Material()
        {
            Diffuse = Color.White;
            Specular = Color.White;
            ShadingType = ShadingType.Diffuse;
            Reflection = 1;
            Refraction = 0;
            RefractionEta = 0.8f;
        }
    }
}
