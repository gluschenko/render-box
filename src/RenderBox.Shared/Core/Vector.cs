using System;
using System.Runtime.CompilerServices;

namespace RenderBox.Core
{
    public class Runtime
    {
        public const MethodImplOptions IMPL_OPTIONS
            = MethodImplOptions.AggressiveInlining
            | MethodImplOptions.AggressiveOptimization;
    }

    public struct Vector2
    {
        public static readonly Vector2 Zero = new(0, 0);
        public static readonly Vector2 One = new(1, 1);
        public static readonly Vector2 Up = new(0, 1);
        public static readonly Vector2 Down = new(0, -1);
        public static readonly Vector2 Right = new(1, 0);
        public static readonly Vector2 Left = new(-1, 0);

        //

        public float x, y;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Vector2(double x, double y)
        {
            this.x = (float)x;
            this.y = (float)y;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Vector2(double x) : this(x, x) { }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Vector2(float x) : this(x, x) { }

        // public
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.x * b.x, a.y * b.y);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.x / b.x, a.y / b.y);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator -(Vector2 a) => a * -1.0f;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator +(Vector2 a) => a;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator *(Vector2 a, float m) => new(a.x * m, a.y * m);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator *(float m, Vector2 a) => a * m;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator /(Vector2 a, float d) => new(a.x / d, a.y / d);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 operator /(float d, Vector2 a) => a / d;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static explicit operator Vector2(Vector4 a) => new(a.x, a.y);

        public float Length => MathHelpres.FastSqrt(x * x + y * y);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public override bool Equals(object obj)
            => EqualsInternal(obj as Vector2?);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        private bool EqualsInternal(Vector2? vector)
            => vector.HasValue && vector.Value == this;


        public override int GetHashCode() => HashCode.Combine(x, y);

    }

    public struct Vector3
    {
        public static readonly Vector3 Zero = new(0, 0, 0);
        public static readonly Vector3 One = new(1, 1, 1);
        public static readonly Vector3 Up = new(0, 1, 0);
        public static readonly Vector3 Down = new(0, -1, 0);
        public static readonly Vector3 Right = new(1, 0, 0);
        public static readonly Vector3 Left = new(-1, 0, 0);
        public static readonly Vector3 Forward = new(0, 0, 1);
        public static readonly Vector3 Back = new(0, 0, -1);

        //

        public float x, y, z;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Vector3(double x, double y, double z)
        {
            this.x = (float)x;
            this.y = (float)y;
            this.z = (float)z;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Vector3(double x) : this(x, x, x) { }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public Vector3(float x) : this(x, x, x) { }

        // public
        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.x / b.x, a.y / b.y, a.z / b.z);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator -(Vector3 a) => a * -1.0f;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator +(Vector3 a) => a;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator *(Vector3 a, float m) => new(a.x * m, a.y * m, a.z * m);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator *(float m, Vector3 a) => a * m;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator /(Vector3 a, float d) => new(a.x / d, a.y / d, a.z / d);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 operator /(float d, Vector3 a) => a / d;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static bool operator ==(Vector3 a, Vector3 b) => a.x == b.x && a.y == b.y && a.z == b.z;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static explicit operator Vector3(Vector4 a) => new(a.x, a.y, a.z);

        public float Length => MathHelpres.FastSqrt(x * x + y * y + z * z);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public override bool Equals(object obj)
            => EqualsInternal(obj as Vector3?);

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        private bool EqualsInternal(Vector3? vector)
            => vector.HasValue && vector.Value == this;

        public override int GetHashCode() => HashCode.Combine(x, y, z);
    }

    public struct Vector4
    {
        public float x, y, z, w;

        public Vector4(double x, double y, double z, double w)
        {
            this.x = (float)x;
            this.y = (float)y;
            this.z = (float)z;
            this.w = (float)w;
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
        private readonly float[] M;

        public Quaternion(bool fill) : this()
        {
            M = new float[16];

            if (!fill) return;

            M[0] = 1.0f; M[4] = 0.0f; M[8] = 0.0f; M[12] = 0.0f;
            M[1] = 0.0f; M[5] = 1.0f; M[9] = 0.0f; M[13] = 0.0f;
            M[2] = 0.0f; M[6] = 0.0f; M[10] = 1.0f; M[14] = 0.0f;
            M[3] = 0.0f; M[7] = 0.0f; M[11] = 0.0f; M[15] = 1.0f;
        }

        public float this[int i]
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

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float Distance(Vector2 a, Vector2 b) => (a - b).Length;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 Lerp(Vector2 a, Vector2 b, float r) => a + (b - a) * r;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 Normalize(Vector2 a) => a / a.Length;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 Reflect(Vector2 i, Vector2 n)
        {
            return i - 2.0f * Dot(n, i) * n;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 Refract(Vector2 i, Vector2 n, float eta)
        {
            var ni = Dot(n, i);
            var k = 1.0f - eta * eta * (1.0f - ni * ni);

            var result = k >= 0.0f
                ? eta * i - n * (eta * ni + MathHelpres.FastSqrt(k))
                : new Vector2();

            return result;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector2 Rotate(Vector2 anchor, Vector2 point, float angle)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            // translate point back to origin:
            point.x -= anchor.x;
            point.y -= anchor.y;

            // rotate point
            var x = (float)(point.x * cos - point.y * sin);
            var y = (float)(point.x * sin + point.y * cos);

            // translate point back:
            point.x = x + anchor.x;
            point.y = y + anchor.y;
            return point;
        }

        #endregion

        #region Vector3

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float Dot(Vector3 a, Vector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static float Distance(Vector3 a, Vector3 b) => (a - b).Length;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 Lerp(Vector3 a, Vector3 b, float r) => a + (b - a) * r;

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            var x = a.y * b.z - a.z * b.y;
            var y = a.z * b.x - a.x * b.z;
            var z = a.x * b.y - a.y * b.x;
            return new(x, y, z);
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 Normalize(Vector3 a)
        {
            return a * (1.0f / a.Length);
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
        public static Vector3 Reflect(Vector3 i, Vector3 n)
        {
            return i - 2 * Dot(i, n) * n;
        }

        [MethodImpl(Runtime.IMPL_OPTIONS)]
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
