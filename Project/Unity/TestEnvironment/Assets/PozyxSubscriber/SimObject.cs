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

        private int refreshRate;

        private int prevIndx;

        private Stopwatch watch;

        private string _id;
        public Tag(string ID, int RefreshRate)
        {
            watch = new Stopwatch();
            watch.Start();
            _id = ID;
            _tagdata = new List<PosData>();
            position = new Vector3D();
            velocity = new Vector3D();
            refreshRate = RefreshRate;
            prevIndx = 0;
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
            if (_tagdata.Count() >= refreshRate)
            {
                bool usedV = false;
                int count = 0;
                Vector3D sum = new Vector3D();
                Vector3D previousPosition = position;
                Vector3D[] Data = new Vector3D[refreshRate];
                int v = _tagdata.Count() - refreshRate;
                for (int i = _tagdata.Count() - 1; i > (_tagdata.Count() - refreshRate - 1); i--)
                {
                    if (_tagdata[i].good)
                    {
                        Data[count] = _tagdata[i].pos;
                        sum += _tagdata[i].pos;
                        count++;
                    }
                }
                if (count == 0)
                {
                    Data[count] += velocity;
                    count++;
                }
                position = normalize(Data, count, sum);
                velocity = (position - previousPosition);
            }
        }

        private Vector3D normalize(Vector3D[] P, int sampleSize, Vector3D sum)
        {
            Vector3D STD = new Vector3D();
            Vector3D mean = sum / sampleSize;

            for (int i = 0; i < sampleSize; i++)
            {
                STD += ((P[i] - mean) * (P[i] - mean));
            }
            STD = STD / (sampleSize - 1);

            Vector3D[] Z = new Vector3D[sampleSize];
            float DataSumx = 0;
            float DataSumxC = 0;
            float DataSumy = 0;
            float DataSumyC = 0;
            float DataSumz = 0;
            float DataSumzC = 0;
            int C = 0;
            for (int i = 0; i < sampleSize; i++)
            {
                Z[i] = (P[i] - mean) / STD;
                if (Z[i].x > 0.002)
                {
                    DataSumx += P[i].x;
                    DataSumxC++;
                }
                if (Z[i].y > 0.002)
                {
                    DataSumy += P[i].y;
                    DataSumyC++;
                }
                if (Z[i].z > 0.002)
                {
                    DataSumz += P[i].z;
                    DataSumzC++;
                }
            }
            Vector3D ret = new Vector3D((DataSumx / DataSumxC),
                                        (DataSumy / DataSumyC),
                                        (DataSumz / DataSumzC));

            //DataSum /= sampleSize;
            return ret;

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
