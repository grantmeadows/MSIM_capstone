using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Reflection;
using System.Runtime.CompilerServices;



[assembly: InternalsVisibleTo("SimEnvironment")]


namespace PozyxSubscriber.Framework
{

    public class Tag
    {
        
        List<PosData> _tagdata;
        Vector3D _position;
        Vector3D _velocity;
        Vector3D _down; //The down vector of the tag in estimated magnitude and direction

        private bool _calibrated;

        private int _refreshRate;

        private int _prevIndx;

        private string _id;
        internal Tag(string ID, int RefreshRate)
        {
            _id = ID;
            _tagdata = new List<PosData>();
            _position = new Vector3D();
            _velocity = new Vector3D();
            _refreshRate = RefreshRate;
            _calibrated = false;
            _prevIndx = 0;
        }

        public string ID { get { return _id; } }

        public Vector3D Position { get { return _position; } }

        public bool isCalibrated
        {
            get { return _calibrated; }
            set { _calibrated = value; }
        }


        public void setRefreshRate(int i) { _refreshRate = i; }


        private PosData GetLatestPosData()
        {
            return _tagdata.Last();
        }



        public void AddData(PosData data)
        {
            _tagdata.Add(data);
            if (_tagdata.Count() >= _refreshRate)
            {
                _calibrated = true;
                bool usedV = false;
                int count = 0;
                Vector3D sum = new Vector3D();
                Vector3D previousPosition = _position;
                Vector3D[] Data = new Vector3D[_refreshRate];
                int v = _tagdata.Count() - _refreshRate;
                for (int i = _tagdata.Count() - 1; i > (_tagdata.Count() - _refreshRate - 1); i--)
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
                    Data[count] += _velocity;
                    count++;
                }
                _position = normalize(Data, count, sum);
                _velocity = (_position - previousPosition);
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
        private string _id;
        private Vector3D _position;
        private Vector3D _orientation;
        private List<Vector3D> O_Vectors;
        private Vector3D O_Vector;
       

        public SimObject(string iD)
        {
            _tags = new List<Tag>();
            _position = new Vector3D(0, 0, 0);
            _orientation = new Vector3D(0, 0, 0);
            O_Vector = new Vector3D();
            O_Vectors = new List<Vector3D>();
            _id = iD;
            _tags = new List<Tag>();
            
        }

        public string ID { get { return _id; } }
        public Vector3D Position
        {
            get
            {
                Update();
                return _position;
            }
        }

        public Vector3D Orientation
        {
            get
            {
                Update();
                return _orientation;
            }
        }


        public void AddTag(Tag tag) { _tags.Add(tag); }

        public void Calibrate()
        {
            Console.Write("Calibrating..  ");
            if (_tags.Count > 0)
            {
                bool ready = false;
                while (!ready)
                {
                    ready = true;
                    foreach (Tag t in _tags)
                    {
                        if (!t.isCalibrated)
                        {
                           ready = false;
                        }
                    }
                }


                if (_tags.Count > 1)
                {
                    Vector3D v = new Vector3D();
                    v = _tags[0].Position - _tags[_tags.Count - 1].Position;
                    O_Vectors.Add(v);

                    Vector3D sum = new Vector3D();
                    int count = 0;
                    foreach (Vector3D vect in O_Vectors)
                    {
                        sum += vect;
                        count++;
                    }
                    O_Vector = sum / count;

                    _orientation = new Vector3D();
                    _position = new Vector3D();
                    Console.WriteLine(_orientation.z);
                }
            }
            Console.WriteLine("Calibration Complete..");
        }

        private void Update()
        {
            Vector3D temp = new Vector3D();
            int count = 0;
            if (_tags.Count > 0)
            {
                foreach (Tag tag in _tags)
                {
                    temp += tag.Position;
                    count++;
                }
                _position = temp / count;
                if (_tags.Count > 1)
                {
                    List<Vector3D> T = new List<Vector3D>();
                    for (int i = 1; i < _tags.Count; i++)
                    {
                        Vector3D v = _tags[0].Position - _tags[i].Position;
                        T.Add(v);
                    }
                    Vector3D sum = new Vector3D();
                    count = 0;
                    foreach (Vector3D vect in T)
                    {
                        sum += vect;
                        count++;

                    }
                    Vector3D final = sum / count;

                    _orientation.z = Vector3D.getAngleZ(O_Vector, final);


                }
            }
        }
    }
}
