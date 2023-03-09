using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PozyxSubscriber.Framework
{
    public class Tag
    {
        List<PosData> _tagdata;
        Vector3D position;
        Vector3D velocity;
        Vector3D down; //The down vector of the tag in estimated magnitude and direction

        int refreshRate;
        private Stopwatch watch;

        private string _id;
        public Tag(string ID)
        {
            watch = new Stopwatch();
            watch.Start();
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

        public Vector3D getPosition()
        {
            return position;
        }

        public void AddData(PosData data)
        {
            _tagdata.Add(data);
            if (watch.ElapsedMilliseconds >= 500)
            {
                bool usedV = false;
                int count = 0;
                Vector3D previousPosition = position;
                Vector3D sum = new Vector3D();
                for (int i = _tagdata.Count() - 1; i > (_tagdata.Count() - 6); i--) {
                    if (_tagdata[i].good)
                    {
                        sum += _tagdata[i].pos;
                        count++;
                    }
                    else if (!usedV)
                    {
                        sum += position + (velocity / 2);
                        usedV = true;
                        count++;
                    }
                }
                position = sum / count;
                velocity = (position - previousPosition) * 2;
            }
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
                x += (float)pos.pos.x;
                y += (float)pos.pos.y;
                z += (float)pos.pos.z;
                count++;
            }
            x /= count;
            y /= count;
            z /= count;
        }
    }
}
