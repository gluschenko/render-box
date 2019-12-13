using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracerSharp
{
    public struct Vector
    {
        public static Vector Zero => new Vector(0, 0, 0);
        public static Vector Up => new Vector(0, 1, 0);
        public static Vector Down => new Vector(0, -1, 0);
        public static Vector Right => new Vector(1, 0, 0);
        public static Vector Left => new Vector(-1, 0, 0);
        public static Vector Forward => new Vector(0, 0, 1);
        public static Vector Back => new Vector(0, 0, -1);

        //

        public float x, y, z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // public
        public static Vector operator +(Vector a, Vector b) => new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector operator -(Vector a, Vector b) => new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector operator *(Vector a, Vector b) => new Vector(a.x * b.x, a.y * b.y, a.z * b.z);
        public static Vector operator /(Vector a, Vector b) => new Vector(a.x / b.x, a.y / b.y, a.z / b.z);

        public static Vector operator *(Vector a, float m) => new Vector(a.x * m, a.y * m, a.z * m);
        public static Vector operator /(Vector a, float d) => new Vector(a.x / d, a.y / d, a.z / d);

        public float Length => (float)Math.Sqrt(x * x + y * y + z * z);

        // static
        public static float Dot(Vector a, Vector b) => a.x * b.x + a.y * b.y + a.z * b.z;
        public static float Distance(Vector a, Vector b) => (a - b).Length;
        public static Vector Lerp(Vector a, Vector b, float r) => a + (b - a) * r;

        public static Vector Normalize(Vector a) => a / a.Length;
    }
}
