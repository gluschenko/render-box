using System;

namespace PathTracerSharp.Rendering
{
    public struct Vector
    {
        public static Vector Zero => new Vector(0, 0, 0);
        public static Vector One => new Vector(1, 1, 1);
        public static Vector Up => new Vector(0, 1, 0);
        public static Vector Down => new Vector(0, -1, 0);
        public static Vector Right => new Vector(1, 0, 0);
        public static Vector Left => new Vector(-1, 0, 0);
        public static Vector Forward => new Vector(0, 0, 1);
        public static Vector Back => new Vector(0, 0, -1);

        //

        public double x, y, z;

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

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

        public static Vector operator -(Vector a) => a * -1.0;
        public static Vector operator +(Vector a) => a;

        public static Vector operator *(Vector a, double m) => new Vector(a.x * m, a.y * m, a.z * m);
        public static Vector operator *(double m, Vector a) => a * m;
        public static Vector operator /(Vector a, double d) => new Vector(a.x / d, a.y / d, a.z / d);
        public static Vector operator /(double d, Vector a) => a / d;

        public double Length => Math.Sqrt(x * x + y * y + z * z);

        // static
        public static double Dot(Vector a, Vector b) => a.x * b.x + a.y * b.y + a.z * b.z;
        public static double Distance(Vector a, Vector b) => (a - b).Length;
        public static Vector Lerp(Vector a, Vector b, float r) => a + (b - a) * r;
        public static Vector Cross(Vector a, Vector b)
        {
            return new Vector(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        public static Vector Normalize(Vector a) => a / a.Length;

        // Vector interactions
        public static Vector Reflect(Vector I, Vector N) 
        { 
            return I - 2 * Dot(I, N) * N; 
        }

        public static Vector Refract(Vector I, Vector N, float ior) 
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
}
