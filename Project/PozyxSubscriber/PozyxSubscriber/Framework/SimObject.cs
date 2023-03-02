using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PozyxSubscriber.Framework
{
    public class Tag
    {
        List<PosData> _tagdata;
        Vector3D position;
        Vector3D velocity;
        Vector3D down;

        int refreshRate;

        private string _id;
        public Tag(string ID)
        {
            _id = ID;
            _tagdata = new List<PosData>();
            position = new Vector3D();
            velocity = new Vector3D();
            refreshRate = 1;
        }

        public void setRefreshRate(int i) { refreshRate = i; }

        public string getID() { return _id; }

        public PosData GetLatestPosData()
        {
            return _tagdata.Last();
        }

        public List<float> getPosition()
        {
            List<float> ret = new List<float>();
            ret.Add(position.x);
            ret.Add(position.y);
            ret.Add(position.z);
            return ret;
        }

        public void AddData(PosData data)
        {
            _tagdata.Add(data);
            //Vector3D Accel = new Vector3D();
            //foreach (var A in data.Acceleration)
            //{
            //    Accel.x += A[0];
            //    Accel.y += A[1];
            //    Accel.z += A[2];
            //}
            //float delta = 1.0f / (float)refreshRate;

            //Accel.x = (Accel.x * data.Acceleration.Count()) / delta;
            //Accel.y = (Accel.y * data.Acceleration.Count()) / delta;
            //Accel.z = (Accel.z * data.Acceleration.Count()) / delta;

            //velocity.x += Accel.x;
            //velocity.y += Accel.y;
            //velocity.z += Accel.z;
            //if (!data.good)
            //{
            //    position.x += velocity.x;
            //    position.y += velocity.y;
            //    position.z += velocity.z;
            //}
        }
    }

    public class SimObject
    {
        private List<Tag> _tags;
        private float x, y, z;
        private float Rotx, Roty, Rotz;

        public SimObject()
        {
            _tags = new List<Tag>();
            x = 0;
            y = 0;
            z = 0;
            Rotx = 0;
            Roty = 0;
            Rotz = 0;
        }


        public void AddTag(Tag tag) { _tags.Add(tag); }

        public List<float> getPosition()
        {
            List<float> position = new List<float>();
            position.Add(x);
            position.Add(y);
            position.Add(z);
            return position;
        }

        public List<float> getOrientation()
        {
            List<float> Orientation = new List<float>();
            Orientation.Add(Rotx);
            Orientation.Add(Roty);
            Orientation.Add(Rotz);
            return Orientation;
        }

        public void Update()
        {
            x = 0;
            y = 0;
            z = 0;
            int count = 0;
            foreach (Tag tag in _tags)
            {
                PosData pos = tag.GetLatestPosData();
                x += (float)pos.x;
                y += (float)pos.y;
                z += (float)pos.z;
                count++;
            }
            x /= count;
            y /= count;
            z /= count;
        }
    }
}
