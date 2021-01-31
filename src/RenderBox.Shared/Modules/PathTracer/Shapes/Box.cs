using System;
using System.Collections.Generic;
using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer.Shapes
{
    public class Box : Mesh
    {
        public Box(Vector3 position, Color diffuse) : base(position, diffuse) 
        {
            double 
                A = 0.5, 
                B = -A;

            var verts = new List<Vector3>()
            {
                new Vector3(B, B, B), // 0
                new Vector3(A, B, B), // 1
                new Vector3(B, B, A), // 2
                new Vector3(A, B, A), // 3
                new Vector3(B, A, B), // 4
                new Vector3(A, A, B), // 5
                new Vector3(B, A, A), // 6
                new Vector3(A, A, A), // 7
            };

            var indices = new List<int>()
            {

            };

            SetData(verts, indices);
        }
    }
}
