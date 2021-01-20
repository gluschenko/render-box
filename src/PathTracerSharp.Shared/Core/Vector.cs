using System;

namespace PathTracerSharp.Core
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
        {
            if (obj is Vector3 vector) 
            {
                return this == vector;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)(x + y * 100 * z + 10000);
        }

        // static
        public static double Dot(Vector3 a, Vector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;
        public static double Distance(Vector3 a, Vector3 b) => (a - b).Length;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float r) => a + (b - a) * r;
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        public static Vector3 Normalize(Vector3 a) => a / a.Length;

        // Vector interactions
        public static Vector3 Reflect(Vector3 I, Vector3 N) 
        { 
            return I - 2 * Dot(I, N) * N; 
        }

        public static Vector3 Refract(Vector3 I, Vector3 N, float ior) 
        { 
            double cosi = MathHelpres.Clamp(Dot(I, N), -1.0, 1.0);
            float etai = 1, etat = ior;
            var n = N; 
            if (cosi < 0) 
            { 
                cosi = -cosi;
            } 
            else 
            {
                (etat, etai) = (etai, etat); 
                n= -N; 
            }

            double eta = etai / etat;
            double k = 1 - eta * eta * (1 - cosi * cosi); 
            return k < 0 ? Zero : eta * I + (eta * cosi - Math.Sqrt(k)) * n; 
        } 
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
        {
            if (obj is Vector2 vector)
            {
                return this == vector;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)(x + y * 100);
        }

        // static
        public static double Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;
        public static double Distance(Vector2 a, Vector2 b) => (a - b).Length;
        public static Vector2 Lerp(Vector2 a, Vector2 b, float r) => a + (b - a) * r;

        public static Vector2 Normalize(Vector2 a) => a / a.Length;
    }

    public struct Point2 
    {
        public int x, y;

        public Point2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}
