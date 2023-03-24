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
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("SimEnvironment")]


namespace PozyxSubscriber.Framework
{

    public class Tag
    {
        /// <summary>
        /// A tag object that is tracked using the Pozyx environment
        /// </summary>
        /// 

        private List<PosData> _tagdata;
        private PozyxVector _position;
        private PozyxVector _velocity;
        private PozyxVector _down; 

        private bool _calibrated;

        private int _refreshRate;

        private int _prevIndx;

        private string _id;

        /// <summary>
        /// Tag Constructor
        /// </summary>
        internal Tag(string ID, int RefreshRate)
        {
            _id = ID;
            _tagdata = new List<PosData>();
            _position = new PozyxVector();
            _velocity = new PozyxVector();
            _refreshRate = RefreshRate;
            _calibrated = false;
            _prevIndx = 0;
        }


        /// <summary>
        /// the ID of the tag
        /// </summary>
        public string ID { get { return _id; } }


        /// <summary>
        /// the current position of the tag
        /// </summary>
        public PozyxVector Position { get { return _position; } }

        public bool isCalibrated
        {
            get { return _calibrated; }
            set { _calibrated = value; }
        }

        /// <summary>
        /// The refresh rate of the tag
        /// </summary>
        public int refreshRate
        {
            get { return _refreshRate; }
            set { _refreshRate = value; }
        }

        /// <summary>
        /// the last positional data of this tag
        /// </summary>
        private PozyxVector LastPosData { get { return _tagdata.Last().pos; } }


        /// <summary>
        /// Adds data to this tag's positional data list, normalizes the data. 
        /// Then calculates the best possible realtime position
        /// </summary>
        /// <param name="data"> the PosData to add to the list</param>
        public void AddData(PosData data)
        {
            _tagdata.Add(data);
            if (_tagdata.Count() >= _refreshRate)
            {
                _calibrated = true;
                bool usedV = false;
                int count = 0;
                PozyxVector sum = new PozyxVector();
                PozyxVector previousPosition = _position;
                PozyxVector[] Data = new PozyxVector[_refreshRate];
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


        private PozyxVector normalize(PozyxVector[] P, int sampleSize, PozyxVector sum)
        {
            PozyxVector STD = new PozyxVector();
            PozyxVector mean = sum / sampleSize;

            for (int i = 0; i < sampleSize; i++)
            {
                STD += ((P[i] - mean) * (P[i] - mean));
            }
            STD = STD / (sampleSize - 1);

            PozyxVector[] Z = new PozyxVector[sampleSize];
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
            PozyxVector ret = new PozyxVector((DataSumx / DataSumxC),
                                        (DataSumy / DataSumyC),
                                        (DataSumz / DataSumzC));

            //DataSum /= sampleSize;
            return ret;

        }


    }



    public class SimObject
    {

        /// <summary>
        /// an object that can be represented by the simulation environment
        /// </summary>

        private List<Tag> _tags;
        private PozyxVector _position;
        private PozyxVector _orientation;
        private List<PozyxVector> O_Vectors;
        private PozyxVector O_Vector;
        private PozyxVector _posoffset;

        /// <summary>
        /// SimObject Constructor
        /// </summary>
        public SimObject()
        {
            _tags = new List<Tag>();
            _position = new PozyxVector(0, 0, 0);
            _orientation = new PozyxVector(0, 0, 0);
            O_Vector = new PozyxVector();
            O_Vectors = new List<PozyxVector>();
            _tags = new List<Tag>();
            
        }


        /// <summary>
        /// PozyxVector: The Position of the current Simobject
        /// </summary>
        public PozyxVector Position
        {
            get
            {
                Update();
                return _position - _posoffset;
            }
        }


        /// <summary>
        /// PozyxVector: The Orientation of the current Simobject
        /// </summary>
        public PozyxVector Orientation
        {
            get
            {
                Update();
                return _orientation;
            }
        }


        /// <summary>
        /// Checks to see if this SimObject has a specific tag attached to it
        /// </summary>
        /// <param name="tag"> The tag to check </param>
        /// <returns> True if tag is attached </returns>
        public bool Contains(Tag tag)
        {
            return _tags.Contains(tag);
        }



        /// <summary>
        /// attaches a tag to this object for tracking, maximum of 2 tags must be attached for orientation measurements
        /// </summary>
        /// <param name="tag"> The tag to attach to the object </param>
        public void AddTag(Tag tag)
        {
            if (_tags.Contains(tag))
                throw new Exception("Tag already attached to this Object");
            else _tags.Add(tag); 
        }


        /// <summary>
        /// binds the SimObject and tags together into a cohesive centeral object, and initializes its coordinate readings.
        /// Sets current real world orientation to 0
        /// Must be done after attaching tags to a SimObject, SimulationEnvironment must be running
        /// </summary>
        public void Calibrate(SimEnvironment S)
        {
            Console.Write("Calibrating..  ");
            if (!S.ConnectedStatus) { throw new Exception("Cannot calibrate if Sim environment is not connected or running"); }
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
                    PozyxVector v = new PozyxVector();
                    v = _tags[0].Position - _tags[_tags.Count - 1].Position;
                    O_Vectors.Add(v);

                    PozyxVector sum = new PozyxVector();
                    int count = 0;
                    foreach (PozyxVector vect in O_Vectors)
                    {
                        sum += vect;
                        count++;
                    }
                    O_Vector = sum / count;
                }
                Update();
                _posoffset = new PozyxVector();
            }
            Console.WriteLine("Calibration Complete..");
        }


        /// <summary>
        /// sets the SimObject's current position in the real world be the set pos values, and reinitializes its coordinate readings.
        /// Sets current real world orientation to 0
        /// Must be done after attaching tags to a SimObject, SimulationEnvironment must be running and connected
        /// </summary>
        /// <param name="S"> The tag to attach to the object </param>
        /// <param name="xpos"> the x origin </param>
        /// <param name="ypos"> the y origin </param>
        /// <param name="zpos"> the z origin </param>
        public void Calibrate(SimEnvironment S, float xpos, float ypos, float zpos)
        {
            Calibrate(S);
            PozyxVector P = new PozyxVector(xpos, ypos, zpos);
            _posoffset += P;
        }


        private void Update()
        {
            PozyxVector temp = new PozyxVector();
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
                    List<PozyxVector> T = new List<PozyxVector>();
                    for (int i = 1; i < _tags.Count; i++)
                    {
                        PozyxVector v = _tags[0].Position - _tags[i].Position;
                        T.Add(v);
                    }
                    PozyxVector sum = new PozyxVector();
                    count = 0;
                    foreach (PozyxVector vect in T)
                    {
                        sum += vect;
                        count++;

                    }
                    PozyxVector final = sum / count;

                    _orientation = PozyxVector.getAngleZ(O_Vector, final);

                    if (final.x < 0) { _orientation.z *= -1; }
                    if (final.z < 0) { _orientation.y *= -1; }
                    if (final.y < 0) { _orientation.x *= -1; }



                }
            }
        }
    }
}
