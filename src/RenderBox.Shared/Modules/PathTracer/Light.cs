using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public class Light
    {
        public Color Color { get; set; }
        public Vector3 Postion { get; set; }
        public double Intensity { get; set; }

        public float ConstantAttenuation { get; set; }
        public float LinearAttenuation { get; set; }
        public float QuadraticAttenuation { get; set; }

        public IShape Shape { get; set; }

        public Light(Color color, Vector3 postion, double intensity, IShape shape)
        {
            Color = color;
            Postion = postion;
            Intensity = intensity;

            ConstantAttenuation = 1;
            LinearAttenuation = 1;
            QuadraticAttenuation = 1;

            Shape = shape;
        }
    }
}
