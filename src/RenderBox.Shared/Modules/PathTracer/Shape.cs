﻿using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public abstract class Shape : IShape
    {
        public Vector3 Position { get; set; }
        public Material Material { get; set; }

        public Shape(Vector3 pos, Color diffuse)
        {
            Position = pos;
            //
            Material = new Material
            {
                Diffuse = diffuse
            };
        }

        public abstract Vector3 CalcNormal(Vector3 pos);
        public abstract bool GetIntersection(Ray ray, double maxDistance, out Hit hit, out double distance);
    }

    public interface IShape
    {
        Vector3 CalcNormal(Vector3 pos);
        bool GetIntersection(Ray ray, double maxDistance, out Hit hit, out double distance);
    }
}