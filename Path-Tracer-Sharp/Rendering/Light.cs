using System;
using PathTracerSharp.Rendering;

namespace PathTracerSharp
{
    public class Light
    {
        public Vector postion;
        public double intensity;

        public Light(Vector postion, double intensity)
        {
            this.postion = postion;
            this.intensity = intensity;
        }
    }
}
