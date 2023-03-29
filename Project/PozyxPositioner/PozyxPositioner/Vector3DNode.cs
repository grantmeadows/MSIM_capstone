using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PozyxPositioner.Framework
{
    public struct PozyxVector
    {
        public float x, y, z;
        public PozyxVector(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static PozyxVector operator +(PozyxVector a, PozyxVector b)
        => new PozyxVector((a.x + b.x), (a.y + b.y), (a.z + b.z));
        public static PozyxVector operator -(PozyxVector a, PozyxVector b)
        => new PozyxVector((a.x - b.x), (a.y - b.y), (a.z - b.z));

        public static PozyxVector operator *(PozyxVector a, PozyxVector b)
        => new PozyxVector((a.x * b.x), (a.y * b.y), (a.z * b.z));

        public static PozyxVector operator /(PozyxVector a, PozyxVector b)
=> new PozyxVector((a.x / b.x), (a.y / b.y), (a.z / b.z));

        public static PozyxVector operator /(PozyxVector a, float b)
        => new PozyxVector((a.x / b), (a.y / b), (a.z / b));
        public static PozyxVector operator *(PozyxVector a, float b)
=> new PozyxVector((a.x * b), (a.y * b), (a.z * b));


        public static float Norm(PozyxVector a)
        {
            float ret =  (float)Math.Sqrt((a.x * a.x) + (a.y * a.y) + (a.z * a.z));
            return ret;
        }

        public static float Dot(PozyxVector a, PozyxVector b)
        {
            float ret = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
            return ret;
        }


        public static PozyxVector getAngleZ(PozyxVector a, PozyxVector b)
        {
            PozyxVector r = new PozyxVector();
            float Denom1 = (float)(Math.Sqrt((a.x * a.x) + (a.y * a.y)));
            float Denom2 = (float)Math.Sqrt((b.x * b.x) + (b.y * b.y));
            r.z = (float)Math.Acos(((a.x * b.x) + (a.y * b.y)) / (Denom1 * Denom2));


            Denom1 = (float)(Math.Sqrt((a.z * a.z) + (a.y * a.y)));
            Denom2 = (float)Math.Sqrt((b.z * b.z) + (b.y * b.y));
            r.x = (float)Math.Acos(((a.z * b.z) + (a.y * b.y)) / (Denom1 * Denom2));



            Denom1 = (float)(Math.Sqrt((a.z * a.z) + (a.x * a.x)));
            Denom2 = (float)Math.Sqrt((b.z * b.z) + (b.x * b.x));
            r.y = (float)Math.Acos(((a.z * b.z) + (a.x * b.x)) / (Denom1 * Denom2));
            return r;

        }


    }



}
