using RenderBox.Core;

namespace RenderBox.Shared.Modules.PathTracer
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public int MaxDepth { get; set; } = 3;
        public double FOV { get; set; } = 90;

        private Vector3 NormalX, NormalY, NormalZ;
        private Quaternion ViewMatrix, PosMatrix, BiasMatrix, ViewPosMatrix, RayMatrix;

        public Camera(Vector3 position)
        {
            NormalX = new Vector3(1.0, 0.0, 0.0);
            NormalY = new Vector3(0.0, 1.0, 0.0);
            NormalZ = new Vector3(0.0, 0.0, 1.0);

            Target = new Vector3(0.0, 0.0, 0.0);
            Position = position;

            BiasMatrix = GetBiasMatrixInverse();
        }

        public void LookAt(Vector3 position, Vector3 target, bool rotateAround)
        {
            Position = position;
            Target = target;

            NormalZ = VectorMath.Normalize(Position - Target);
            NormalX = VectorMath.Normalize(VectorMath.Cross(new Vector3(0.0f, 1.0f, 0.0f), NormalZ));
            NormalY = VectorMath.Cross(NormalZ, NormalX);

            if (!rotateAround)
            {
                Target = Position;
                Position += NormalZ * 0.05f;
            }

            CalculateRayMatrix();
        }

        public void CalculateRayMatrix()
        {
            ViewMatrix[0] = NormalX.x; ViewMatrix[4] = NormalY.x; ViewMatrix[8] = NormalZ.x;
            ViewMatrix[1] = NormalX.y; ViewMatrix[5] = NormalY.y; ViewMatrix[9] = NormalZ.y;
            ViewMatrix[2] = NormalX.z; ViewMatrix[6] = NormalY.z; ViewMatrix[10] = NormalZ.z;

            RayMatrix = ViewMatrix * PosMatrix * BiasMatrix * ViewPosMatrix;
        }

        private Quaternion GetBiasMatrix()
        {
            var B = new Quaternion(false);

            B[0] = 0.5f; B[4] = 0.0f; B[8] = 0.0f; B[12] = 0.5f;
            B[1] = 0.0f; B[5] = 0.5f; B[9] = 0.0f; B[13] = 0.5f;
            B[2] = 0.0f; B[6] = 0.0f; B[10] = 0.5f; B[14] = 0.5f;
            B[3] = 0.0f; B[7] = 0.0f; B[11] = 0.0f; B[15] = 1.0f;

            return B;
        }

        private Quaternion GetBiasMatrixInverse()
        {
            var BI = new Quaternion(false);

            BI[0] = 2.0f; BI[4] = 0.0f; BI[8] = 0.0f; BI[12] = -1.0f;
            BI[1] = 0.0f; BI[5] = 2.0f; BI[9] = 0.0f; BI[13] = -1.0f;
            BI[2] = 0.0f; BI[6] = 0.0f; BI[10] = 2.0f; BI[14] = -1.0f;
            BI[3] = 0.0f; BI[7] = 0.0f; BI[11] = 0.0f; BI[15] = 1.0f;

            return BI;
        }
    }
}
