﻿using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public class Light
    {
        public Vector3 Postion { get; set; }
        public double Intensity { get; set; }
        public IShape Shape { get; set; }

        public Light(Vector3 postion, double intensity, IShape shape)
        {
            Postion = postion;
            Intensity = intensity;
            Shape = shape;
        }
    }
}