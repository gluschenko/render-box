using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
