using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PozyxSubscriber.Framework
{
    public class Anchor
    {
        PosData _position;
        string _id;

        public Anchor(string ID)
        {
            _id = ID;
        }

        public void SetPosition(float x, float y, float z)
        {
            _position = new PosData(x, y, z);
        }
        public PosData getPosition() { return _position; }

    }
}
