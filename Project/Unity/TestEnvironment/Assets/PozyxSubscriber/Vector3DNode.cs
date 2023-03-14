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
    }



}
