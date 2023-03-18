using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PozyxSubscriber.Framework
{
    public struct Vector3D
    {
        public float x, y, z;
        public Vector3D()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public Vector3D(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        => new Vector3D((a.x + b.x), (a.y + b.y), (a.z + b.z));
        public static Vector3D operator -(Vector3D a, Vector3D b)
        => new Vector3D((a.x - b.x), (a.y - b.y), (a.z - b.z));

        public static Vector3D operator *(Vector3D a, Vector3D b)
        => new Vector3D((a.x * b.x), (a.y * b.y), (a.z * b.z));

        public static Vector3D operator /(Vector3D a, Vector3D b)
=> new Vector3D((a.x / b.x), (a.y / b.y), (a.z / b.z));

        public static Vector3D operator /(Vector3D a, float b)
        => new Vector3D((a.x / b), (a.y / b), (a.z / b));
        public static Vector3D operator *(Vector3D a, float b)
=> new Vector3D((a.x * b), (a.y * b), (a.z * b));


        public static float Norm(Vector3D a)
        {
            float ret =  (float)Math.Sqrt((a.x * a.x) + (a.y * a.y) + (a.z * a.z));
            return ret;
        }

        public static float Dot(Vector3D a, Vector3D b)
        {
            float ret = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
            return ret;
        }


        public static float getAngleZ(Vector3D a, Vector3D b)
        {
            float Denom1 = (float)(Math.Sqrt((a.x * a.x) + (a.y * a.y)));
            float Denom2 = (float)Math.Sqrt((b.x * b.x) + (b.y * b.y));
            float r = (float)Math.Acos(((a.x * b.x) + (a.y * b.y)) / (Denom1 * Denom2));// * (float)(180 / Math.PI);

            Console.WriteLine(r);
            return r;

        }


    }



}
