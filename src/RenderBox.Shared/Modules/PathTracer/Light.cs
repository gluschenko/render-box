using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public class Light
    {
        public Vector3 postion;
        public double intensity;

        public Light(Vector3 postion, double intensity)
        {
            this.postion = postion;
            this.intensity = intensity;
        }
    }
}
