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
        public void Initialize(string host, int port, int numObjects, int numTags, string filename, int refreshRate)
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
            _refreshRate = refreshRate;
            _MqqtClient = new MqttClient(numTags, host, port, this, filename);       
        }

        public void Initialize(string filename, int refreshRate)
        {
            _objects = new List<SimObject>();
            _mutex = true;
            _host = "";
            _port = 0;
            _tags = new Dictionary<string, Tag>();
            _tagIDs = new List<string>();
            _anchors = new Dictionary<string, Anchor>();
            _anchorIDs = new List<string>();
            _refreshRate = refreshRate;
            _reader = new Reader(filename, this);
        }




        Reader? _reader;
        private static MqttClient? _MqqtClient;
        List<SimObject> _objects;
        Dictionary<string, Tag> _tags;
        List<string> _tagIDs;

        Dictionary<string, Anchor> _anchors;
        List<string> _anchorIDs;

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

        public void newTag(string ID, int refreshRate)
        {
            _tagIDs.Add(ID);
            _tags[ID] = new Tag(ID, _refreshRate);
            _tags[ID].AddData(new PosData());
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

        public Dictionary<string, Vector3D> getAllPositions()
        {
            Dictionary<string, Vector3D> ret = new Dictionary<string, Vector3D>();
            foreach (var T in _tagIDs)
            {
                ret[T] = _tags[T].getPosition();
            }
            return ret;
        }

        public Anchor getAnchor(string ID) { return _anchors[ID]; }

        public Tag GetTag(string ID) { return _tags[ID]; }

        public void SetAnchor(string ID, Anchor anchor) { _anchors["ID"] = anchor; }



        public List<string> GetTagIDs() { return _tagIDs; }

    }



}
