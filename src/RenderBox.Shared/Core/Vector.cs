using System;
using System.Runtime.CompilerServices;

namespace RenderBox.Core
{
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(double x) : this(x, x) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(float x) : this(x, x) { }

        // public
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator -(Vector2 a) => a * -1.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator +(Vector2 a) => a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(Vector2 a, double m) => new Vector2(a.x * m, a.y * m);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *(double m, Vector2 a) => a * m;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 a, double d) => new Vector2(a.x / d, a.y / d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(double d, Vector2 a) => a / d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector2(Vector4 a) => new Vector2(a.x, a.y);

        public double Length => MathHelpres.FastSqrt(x * x + y * y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
            => EqualsInternal(obj as Vector2?);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EqualsInternal(Vector2? vector)
            => vector.HasValue && vector.Value == this;


        public override int GetHashCode() => HashCode.Combine(x, y);

    }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(double x) : this(x, x, x) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(float x) : this(x, x, x) { }

        // public
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a) => a * -1.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a) => a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, double m) => new Vector3(a.x * m, a.y * m, a.z * m);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(double m, Vector3 a) => a * m;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, double d) => new Vector3(a.x / d, a.y / d, a.z / d);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(double d, Vector3 a) => a / d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 a, Vector3 b) => a.x == b.x && a.y == b.y && a.z == b.z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector3(Vector4 a) => new Vector3(a.x, a.y, a.z);

        public double Length => MathHelpres.FastSqrt(x * x + y * y + z * z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
            => EqualsInternal(obj as Vector3?);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EqualsInternal(Vector3? vector)
            => vector.HasValue && vector.Value == this;

        public override int GetHashCode() => HashCode.Combine(x, y, z);
    }

    public struct Vector4
    {
        public double x, y, z, w;

        public Vector4(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

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

    public struct Quaternion
    {
        private readonly double[] M;

        public Quaternion(bool fill) : this()
        {
            M = new double[16];

            if (!fill) return;

            M[0] = 1.0f; M[4] = 0.0f; M[8] = 0.0f; M[12] = 0.0f;
            M[1] = 0.0f; M[5] = 1.0f; M[9] = 0.0f; M[13] = 0.0f;
            M[2] = 0.0f; M[6] = 0.0f; M[10] = 1.0f; M[14] = 0.0f;
            M[3] = 0.0f; M[7] = 0.0f; M[11] = 0.0f; M[15] = 1.0f;
        }

        public double this[int i]
        {
            get
            {
                return M[i];
            }
            set
            {
                M[i] = value;
            }
        }

        public double this[int x, int y] => M[y * 4 + x];

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            var c = new Quaternion(false);

            c.M[0] = a.M[0] * b.M[0] + a.M[4] * b.M[1] + a.M[8] * b.M[2] + a.M[12] * b.M[3];
            c.M[1] = a.M[1] * b.M[0] + a.M[5] * b.M[1] + a.M[9] * b.M[2] + a.M[13] * b.M[3];
            c.M[2] = a.M[2] * b.M[0] + a.M[6] * b.M[1] + a.M[10] * b.M[2] + a.M[14] * b.M[3];
            c.M[3] = a.M[3] * b.M[0] + a.M[7] * b.M[1] + a.M[11] * b.M[2] + a.M[15] * b.M[3];

            c.M[4] = a.M[0] * b.M[4] + a.M[4] * b.M[5] + a.M[8] * b.M[6] + a.M[12] * b.M[7];
            c.M[5] = a.M[1] * b.M[4] + a.M[5] * b.M[5] + a.M[9] * b.M[6] + a.M[13] * b.M[7];
            c.M[6] = a.M[2] * b.M[4] + a.M[6] * b.M[5] + a.M[10] * b.M[6] + a.M[14] * b.M[7];
            c.M[7] = a.M[3] * b.M[4] + a.M[7] * b.M[5] + a.M[11] * b.M[6] + a.M[15] * b.M[7];

            c.M[8] = a.M[0] * b.M[8] + a.M[4] * b.M[9] + a.M[8] * b.M[10] + a.M[12] * b.M[11];
            c.M[9] = a.M[1] * b.M[8] + a.M[5] * b.M[9] + a.M[9] * b.M[10] + a.M[13] * b.M[11];
            c.M[10] = a.M[2] * b.M[8] + a.M[6] * b.M[9] + a.M[10] * b.M[10] + a.M[14] * b.M[11];
            c.M[11] = a.M[3] * b.M[8] + a.M[7] * b.M[9] + a.M[11] * b.M[10] + a.M[15] * b.M[11];

            c.M[12] = a.M[0] * b.M[12] + a.M[4] * b.M[13] + a.M[8] * b.M[14] + a.M[12] * b.M[15];
            c.M[13] = a.M[1] * b.M[12] + a.M[5] * b.M[13] + a.M[9] * b.M[14] + a.M[13] * b.M[15];
            c.M[14] = a.M[2] * b.M[12] + a.M[6] * b.M[13] + a.M[10] * b.M[14] + a.M[14] * b.M[15];
            c.M[15] = a.M[3] * b.M[12] + a.M[7] * b.M[13] + a.M[11] * b.M[14] + a.M[15] * b.M[15];

            return c;
        }

        public static Vector2 operator *(Quaternion a, Vector2 b) => (Vector2)(a * new Vector4(b.x, b.y, 0.0f, 1.0f));
        public static Vector3 operator *(Quaternion a, Vector3 b) => (Vector3)(a * new Vector4(b.x, b.y, b.z, 1.0f));
        public static Vector4 operator *(Quaternion a, Vector4 b)
        {
            Vector4 v;

            v.x = a.M[0] * b.x + a.M[4] * b.y + a.M[8] * b.z + a.M[12] * b.w;
            v.y = a.M[1] * b.x + a.M[5] * b.y + a.M[9] * b.z + a.M[13] * b.w;
            v.z = a.M[2] * b.x + a.M[6] * b.y + a.M[10] * b.z + a.M[14] * b.w;
            v.w = a.M[3] * b.x + a.M[7] * b.y + a.M[11] * b.z + a.M[15] * b.w;

            return v;
        }

        public static bool operator ==(Quaternion a, Quaternion b) => a.M == b.M;
        public static bool operator !=(Quaternion a, Quaternion b) => !(a == b);


        public override bool Equals(object obj)
            => EqualsInternal(obj as Quaternion?);

        private bool EqualsInternal(Quaternion? vector)
            => vector.HasValue && vector.Value == this;

        public override int GetHashCode() => HashCode.Combine(M);
    }

    public static class VectorMath
    {
        #region Vector2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(Vector2 a, Vector2 b) => (a - b).Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(Vector2 a, Vector2 b, float r) => a + (b - a) * r;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalize(Vector2 a) => a / a.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Reflect(Vector2 i, Vector2 n)
        {
            return i - 2.0f * Dot(n, i) * n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Refract(Vector2 i, Vector2 n, float eta)
        {
            var ni = Dot(n, i);
            var k = 1.0f - eta * eta * (1.0f - ni * ni);

            var result = k >= 0.0f
                ? eta * i - n * (eta * ni + MathHelpres.FastSqrt(k))
                : new Vector2();

            return result;
        }

        #endregion

        #region Vector3

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(Vector3 a, Vector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(Vector3 a, Vector3 b) => (a - b).Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(Vector3 a, Vector3 b, float r) => a + (b - a) * r;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            var x = a.y * b.z - a.z * b.y;
            var y = a.z * b.x - a.x * b.z;
            var z = a.x * b.y - a.y * b.x;
            return new Vector3(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalize(Vector3 a)
        {
            return a * (1.0 / a.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Reflect(Vector3 i, Vector3 n)
        {
            return i - 2 * Dot(i, n) * n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Refract(Vector3 i, Vector3 n, float eta)
        {
            var ni = Dot(n, i);
            var k = 1.0f - eta * eta * (1.0f - ni * ni);

            var result = k >= 0.0f
                ? eta * i - n * (eta * ni + MathHelpres.FastSqrt(k))
                : new Vector3();

            return result;
        }

        #endregion
    }
}
