using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public enum ShadingType
    {
        Diffuse = 0,
        DiffuseGlossy = 1,
        Reflection = 2,
        ReflectionAndRefraction = 3,
    }

    public class Material
    {
        public Color Ambient { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }
        public ShadingType ShadingType { get; set; }

        public Material()
        {
            Ambient = Color.White;
            Diffuse = Color.White;
            Specular = Color.White;
            ShadingType = ShadingType.Diffuse;
        }
    }
}
