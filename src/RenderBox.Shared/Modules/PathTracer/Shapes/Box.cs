using System.Collections.Generic;
using System.Linq;
using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer.Shapes
{
    public class Box : Mesh
    {
        public Vector3 Scale { get; set; }

        public Box(Vector3 position, Color diffuse, Vector3? scale = null) : base(position, diffuse)
        {
            scale ??= new Vector3(1, 1, 1);
            Scale = scale.Value;

            double A = 0.5, B = -A;

            var verts = new List<Vector3>()
            {
                new Vector3(A, B, A), // 0
                new Vector3(A, B, B), // 1
                new Vector3(B, B, A), // 2
                new Vector3(B, B, B), // 3
                new Vector3(A, A, A), // 4
                new Vector3(A, A, B), // 5
                new Vector3(B, A, A), // 6
                new Vector3(B, A, B), // 7
            };

            var indices = new List<int>()
            {
                // Top
                4, 5, 6,
                7, 6, 5,

                // Bottom
                1, 2, 3,
                2, 1, 0,

                // Front
                0, 1, 5,
                4, 0, 5,

                // Back
                7, 3, 2,
                2, 6, 7,

                // Left
                1, 3, 7,
                7, 5, 1,

                // Right
                6, 2, 0,
                0, 4, 6,
            };

            SetData(verts.Select(x => x * scale.Value).ToList(), indices);
        }

        public override Vector3 GetLightEmission(Vector3 random)
        {
            return Scale * random;
        }
    }
}
