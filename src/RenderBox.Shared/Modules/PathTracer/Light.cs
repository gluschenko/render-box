using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public class Light
    {
        public Color Color { get; set; }
        public double Intensity { get; set; }

        public float ConstantAttenuation { get; set; }
        public float LinearAttenuation { get; set; }
        public float QuadraticAttenuation { get; set; }

        public IShape Shape { get; set; }

        public Light(Color color, double intensity)
        {
            Color = color;
            Intensity = intensity;

            ConstantAttenuation = 1;
            LinearAttenuation = 0.5f;
            QuadraticAttenuation = 0.5f;
        }
    }
}
