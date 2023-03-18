using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Data;
using PozyxSubscriber.Framework;

namespace PozyxSubscriber
{

    /// <summary>
    /// Simulation enviornemnt
    /// </summary>
    public sealed class SimEnvironment
    {

        private static SimEnvironment? _instance = null;
        private bool _connectedStatus;

        private bool reader;

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
        public void Initialize(string host, int port, int numObjects, int numTags, string filename, int refreshRate)
        {
            //_objects = new Dictionary<string, SimObject>();
            //_objectIDs = new List<string>();
            reader = false;
            _host = host;
            _port = port;
            _tags = new Dictionary<string, Tag>();
            _tagIDs = new List<string>();
            _reader = null;
            _refreshRate = refreshRate;
            _MqqtClient = new MqttClient(numTags, host, port, this, filename);       
        }

        public void Initialize(string filename, int refreshRate)
        {
            reader = true;
            //_objects = new Dictionary<string, SimObject>();
            //_objectIDs = new List<string>();
            _host = "";
            _port = 0;
            _tags = new Dictionary<string, Tag>();
            _tagIDs = new List<string>();
            _refreshRate = refreshRate;
            _reader = new Reader(filename, this);
        }

        public void StartEnvironment()
        {
            if (reader) _reader.Begin();
            else _MqqtClient.Begin();
        }


        Reader? _reader;
        private static MqttClient? _MqqtClient;
        
        Dictionary<string, Tag> _tags;
        List<string> _tagIDs;

        private string _host;
        private int _port;

        int _refreshRate;

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
                bool Success = M["success"].Value<bool>();
                if (Success)
                {
                    x = M["data"]["coordinates"]["x"].Value<float>();
                    y = M["data"]["coordinates"]["y"].Value<float>();
                    z = M["data"]["coordinates"]["z"].Value<float>();
                }
                else
                {
                    x = 0; y = 0; z = 0;
                }
                PosData newData = new PosData(x, y, z);
                newData.good = Success;
                var Accel = M["data"]["tagData"]["accelerometer"];
                if (Accel.Count() > 0)
                {
                    var temp = Accel.First();
                    newData.Acceleration = new List<Vector3D>();
                    for (int i = 0; i < Accel.Count(); i++)
                    {
                        Vector3D vect;
                        vect.x = temp[0].Value<float>();
                        vect.y = temp[1].Value<float>();
                        vect.z = temp[2].Value<float>();
                        newData.Acceleration.Add(vect);
                        temp = temp.Next;
                    }
                }
                else
                {
                    newData.Acceleration = new List<Vector3D>();
                    newData.Acceleration.Add(new Vector3D());
                }


                if (_tags.ContainsKey(ID))
                        _tags[ID].AddData(newData);
                    else
                    {
                        _tagIDs.Add(ID);
                        _tags[ID] = new Tag(ID, _refreshRate);
                        _tags[ID].AddData(newData);
                    }

                


            }
        }

        public Tag newTag(string ID, int refreshRate)
        {
            if (_tags.ContainsKey(ID))
            {
                throw new Exception("ID declared twice within Pozyx Environment");
            }
            _tagIDs.Add(ID);
            _tags[ID] = new Tag(ID, refreshRate);
            _tags[ID].AddData(new PosData());
            return _tags[ID];
        }

        //public void newTag(Tag T)
        //{
        //    string ID = T.ID;
        //    _tagIDs.Add(ID);
        //    _tags[ID] = T;
        //    _tags[ID].AddData(new PosData());
        //}

        public Tag RemoveTag(string ID)
        {
            Tag T = _tags[ID];
            _tagIDs.Remove(ID);
            _tags.Remove(ID);
            return T;
        }


        //public void newObject(SimObject S)
        //{
        //    string ID = S.ID;
        //    _objectIDs.Add(ID);
        //    _objects[ID] = S;
        //}

        //public SimObject createObject(string ID)
        //{
        //    SimObject S = new SimObject(ID);
        //    _objectIDs.Add(ID);
        //    _objects[ID] = S;
        //    return S;
        //}

        public Tag GetTag(string ID) { return _tags[ID]; }

        //public SimObject getObject(string ID) { return _objects[ID];}

        public List<string> TagIDs { get { return _tagIDs; } }
        //public List<string> ObjectIDs { get { return _objectIDs; } }


    }



}
