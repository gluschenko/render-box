using System;

namespace RenderBox.Core
{
    public struct Vector3
    {
        public static Vector3 Zero => new Vector3(0, 0, 0);
        public static Vector3 One => new Vector3(1, 1, 1);
        public static Vector3 Up => new Vector3(0, 1, 0);
        public static Vector3 Down => new Vector3(0, -1, 0);
        public static Vector3 Right => new Vector3(1, 0, 0);
        public static Vector3 Left => new Vector3(-1, 0, 0);
        public static Vector3 Forward => new Vector3(0, 0, 1);
        public static Vector3 Back => new Vector3(0, 0, -1);

        //

        public double x, y, z;

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(double x) : this(x, x, x) { }
        public Vector3(float x) : this(x, x, x) { }

        // public
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

        public static Vector3 operator -(Vector3 a) => a * -1.0;
        public static Vector3 operator +(Vector3 a) => a;

        public static Vector3 operator *(Vector3 a, double m) => new Vector3(a.x * m, a.y * m, a.z * m);
        public static Vector3 operator *(double m, Vector3 a) => a * m;
        public static Vector3 operator /(Vector3 a, double d) => new Vector3(a.x / d, a.y / d, a.z / d);
        public static Vector3 operator /(double d, Vector3 a) => a / d;

        public static bool operator ==(Vector3 a, Vector3 b) => a.x == b.x && a.y == b.y && a.z == b.z;
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        public double Length => Math.Sqrt(x * x + y * y + z * z);

        public override bool Equals(object obj) 
            => EqualsInternal(obj as Vector3?);

        private bool EqualsInternal(Vector3? vector) 
            => vector.HasValue && vector.Value == this;

        public override int GetHashCode() => HashCode.Combine(x, y, z);

    }

    public struct Vector2
    {
        public static Vector2 Zero => new Vector2(0, 0);
        public static Vector2 One => new Vector2(1, 1);
        public static Vector2 Up => new Vector2(0, 1);
        public static Vector2 Down => new Vector2(0, -1);
        public static Vector2 Right => new Vector2(1, 0);
        public static Vector2 Left => new Vector2(-1, 0);

        //

        public double x, y;

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(double x) : this(x, x) { }
        public Vector2(float x) : this(x, x) { }

        // public
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
        public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

        public static Vector2 operator -(Vector2 a) => a * -1.0;
        public static Vector2 operator +(Vector2 a) => a;

        public static Vector2 operator *(Vector2 a, double m) => new Vector2(a.x * m, a.y * m);
        public static Vector2 operator *(double m, Vector2 a) => a * m;
        public static Vector2 operator /(Vector2 a, double d) => new Vector2(a.x / d, a.y / d);
        public static Vector2 operator /(double d, Vector2 a) => a / d;

        public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

        public double Length => Math.Sqrt(x * x + y * y);

        public override bool Equals(object obj) 
            => EqualsInternal(obj as Vector2?);

        private bool EqualsInternal(Vector2? vector) 
            => vector.HasValue && vector.Value == this;

        public override int GetHashCode() => HashCode.Combine(x, y);

    }

    public struct Point2
    {
        public int x, y;

        public Point2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode() => HashCode.Combine(x, y);
    }

    public static class VectorMath 
    {
        #region Vector2

        public static double Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;
        public static double Distance(Vector2 a, Vector2 b) => (a - b).Length;
        public static Vector2 Lerp(Vector2 a, Vector2 b, float r) => a + (b - a) * r;
        public static Vector2 Normalize(Vector2 a) => a / a.Length;

        public static Vector2 Reflect(Vector2 i, Vector2 n)
        {
            return i - 2.0f * Dot(n, i) * n;
        }

        public static Vector2 Refract(Vector2 i, Vector2 n, float eta)
        {
            var ni = Dot(n, i);
            var k = 1.0f - eta * eta * (1.0f - ni * ni);

            var result = k >= 0.0f
                ? eta * i - n * (eta * ni + Math.Sqrt(k))
                : new Vector2();

            return result;
        }

        #endregion

        #region Vector3

        public static double Dot(Vector3 a, Vector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;
        public static double Distance(Vector3 a, Vector3 b) => (a - b).Length;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float r) => a + (b - a) * r;
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            var x = a.y * b.z - a.z * b.y;
            var y = a.z * b.x - a.x * b.z;
            var z = a.x * b.y - a.y * b.x;
            return new Vector3(x, y, z);
        }

        public static Vector3 Normalize(Vector3 a) => a / a.Length;

        public static Vector3 Reflect(Vector3 i, Vector3 n)
        {
            return i - 2 * Dot(i, n) * n;
        }

        public static Vector3 Refract(Vector3 i, Vector3 n, float eta)
        {
            var ni = Dot(n, i);
            var k = 1.0f - eta * eta * (1.0f - ni * ni);

            var result = k >= 0.0f 
                ? eta * i - n * (eta * ni + Math.Sqrt(k)) 
                : new Vector3();

            return result;
        }

        #endregion
    }
}
