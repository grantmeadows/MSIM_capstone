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
        public float? x, y, z;
        public bool good;
        public List<List<float>>? Acceleration;
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
            x = _x;
            y = _y;
            z = _z;
            Acceleration = null;
        }
        public PosData()
        {
            good = false;
            x = 0;
            y = 0;
            z = 0;
            Acceleration = null;
        }
    }
}
