using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using mqtt_c;
using System.Threading;
using Newtonsoft.Json.Linq;
using filereader;
using System.Data;

namespace SimulationEnviornment
{

    /// <summary>
    /// Simulation enviornemnt
    /// </summary>
    public sealed class SimEnvironment
    {

        private bool _mutex;
        private static SimEnvironment? _instance = null;
        private bool _connectedStatus;

        public SimEnvironment()
        {
        }

        public static SimEnvironment Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SimEnvironment();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Simulation Enviorntment Constructor, creates MqqtClient instance
        /// starting subscription to tags topic
        /// </summary>
        /// <param name="host">Local IP addres of Pozyx gateway</param>
        /// <param name="port">Port</param>
        /// <param name="numObjects">Number of objects in enviornment</param>
        /// <param name="numTags">Number of tags in enviornment</param>
        public void Initialize(string host, int port, int numObjects, int numTags)
        {
            _objects = new List<SimObject>();
            _mutex = true;
            _host = host;
            _port = port;
            _tags = new Dictionary<string, Tag>();
            _tagIDs = new List<string>();
            _anchors = new Dictionary<string, Anchor>();
            _anchorIDs = new List<string>();
            _reader = null;
            _MqqtClient = new MqttClient(numTags, host, port, this);

        }

        public void Initialize(string filename)
        {
            _objects = new List<SimObject>();
            _mutex = true;
            _host = "";
            _port = 0;
            _tags = new Dictionary<string, Tag>();
            _tagIDs = new List<string>();
            _anchors = new Dictionary<string, Anchor>();
            _anchorIDs = new List<string>();
            _reader = new filereader.Reader(filename, this);

        }


        filereader.Reader? _reader;
        private static MqttClient? _MqqtClient;
        List<SimObject> _objects;
        Dictionary<string, Tag> _tags;
        List<string> _tagIDs;

        Dictionary<string, Anchor> _anchors;
        List<string> _anchorIDs;

        private string _host;
        private int _port;

        public bool ConnectedStatus
        {
            get { return _connectedStatus; }
            set { _connectedStatus = value; }
        }



        public void PushData(JArray msgdata)
        {
            foreach (var M in msgdata)
            {
                float x = 0;
                float y = 0;
                float z = 0;
                string ID = M["tagId"].Value<string>();
                if (M["success"].Value<bool>())
                {

                    x = M["data"]["coordinates"]["x"].Value<float>();
                    y = M["data"]["coordinates"]["y"].Value<float>();
                    z = M["data"]["coordinates"]["z"].Value<float>();
                    PosData newData = new PosData(x, y, z);
                    newData.good = M["success"].Value<bool>();

                    if (_tags.ContainsKey(ID))
                        _tags[ID].AddData(newData);
                    else
                    {
                        _tagIDs.Add(ID);
                        _tags[ID] = new Tag(ID);
                        _tags[ID].AddData(newData);
                    }

                }


            }
        }

        private void MutexLock()
        {
            while (_mutex) Thread.Sleep(100);
            _mutex = true;
        }
        private void MutexUnlock()
        {
            _mutex = false;
        }


        public PosData getLatestposition(string ID) { if (_tags.ContainsKey(ID)) return _tags[ID].GetLatestPosData(); else return new PosData(0, 0, 0); }

        public Dictionary<string, PosData> getAllPositions()
        {
            Dictionary<string, PosData> ret = new Dictionary<string, PosData>();
            foreach (var T in _tagIDs)
            {
                ret[T] = _tags[T].GetLatestPosData();
            }

            return ret;
        }

        public Anchor getAnchor(string ID) { return _anchors[ID]; }

        public Tag GetTag(string ID) { return _tags[ID]; }

        public void SetAnchor(string ID, Anchor anchor) { _anchors["ID"] = anchor; }



        public List<string> GetTagIDs() { return _tagIDs; }

    }

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


    public struct Vector3D
    {
        public float x, y, z;
        public Vector3D()
        {
            x = 0;
            y = 0;
            z = 0;
        }
    }

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
