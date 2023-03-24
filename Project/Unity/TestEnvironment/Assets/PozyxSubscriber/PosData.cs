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
    public class PosData
    {
        public PozyxVector pos;
        public bool good;
        public List<PozyxVector> Acceleration;


        /// <summary>
        /// Create position data node with x, y, z coordinates
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_z"></param>
        public PosData(float _x, float _y, float _z)
        {
            good = true;
            pos.x = _x;
            pos.y = _y;
            pos.z = _z;
            Acceleration = new List<PozyxVector>(); ;
        }
        public PosData()
        {
            good = false;
            pos.x = 0;
            pos.y = 0;
            pos.z = 0;
            Acceleration = new List<PozyxVector>();
        }
    }
}
