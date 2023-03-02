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

namespace PozyxSubscriber.Framework
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



}
