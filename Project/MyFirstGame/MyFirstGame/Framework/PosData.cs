using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PozyxSubscriber.Framework
{
    /// <summary>
    /// Contains a set of position data
    /// </summary>
    public struct PosData
    {
        public Vector3D pos;
        public bool good;
        public List<Vector3D> Acceleration;
        /// <summary>
        /// Create position data node with x, y, z coordinates
        /// </summary>
        /// <param name="_ID"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_z"></param>
        public PosData(float _x, float _y, float _z)
        {
            good = true;
            pos.x = _x;
            pos.y = _y;
            pos.z = _z;
            Acceleration = new List<Vector3D>(); ;
        }
        public PosData()
        {
            good = false;
            pos.x = 0;
            pos.y = 0;
            pos.z = 0;
            Acceleration = new List<Vector3D>();
        }
    }
}
