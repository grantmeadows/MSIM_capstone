using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using mqtt_c;
using Newtonsoft.Json.Linq;

namespace SimulationEnviornment
{

    /// <summary>
    /// Simulation enviornemnt
    /// </summary>
    public class SimEnviornment
    {

        private bool _mutex;

        /// <summary>
        /// Simulation Enviorntment Constructor, creates MqqtClient instance
        /// starting subscription to tags topic
        /// </summary>
        /// <param name="host">Local IP addres of Pozyx gateway</param>
        /// <param name="port">Port</param>
        /// <param name="numObjects">Number of objects in enviornment</param>
        /// <param name="numTags">Number of tags in enviornment</param>
        public SimEnviornment(string host, int port, int numObjects, int numTags)
        {
            _objects = new List<SimObject>();
            _mutex = true;
            _host = host;
            _port = port;
            _MqqtClient = new MqttClient(numTags, host, port, this);
            
        }

        public void PushData(JArray msgdata)
        {
            foreach (var M in msgdata)
            {
               
                if (M["success"].Value<string>() == "true")
                {
                    string ID = M["tagId"].Value<string>();
                    float x = M["data"]["coordinates"]["x"].Value<float>();
                    float y = M["data"]["coordinates"]["y"].Value<float>();
                    float z = M["data"]["coordinates"]["z"].Value<float>();
                    PosData newData = new PosData(x, y, z);

                    
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


        public PosData getLatestposition(string ID)
        {
            
            return _tags[ID].GetLatestPosData();
           
        }

        public Dictionary<string, PosData> getAllPositions()
        {
            Dictionary<string, PosData> ret = new Dictionary<string, PosData>();
            foreach (var T in _tagIDs)
            {
                ret[T] = _tags[T].GetLatestPosData();
            }
           
            return ret;
        }

        public Anchor getAnchor(string ID)
        {
            return _anchors[ID];
        }

        public Tag GetTag(string ID) { return _tags[ID]; }

        public void SetAnchor(string ID, Anchor anchor)
        {
            _anchors["ID"] = anchor;
        }

               

        private static MqttClient? _MqqtClient;
        List<SimObject> _objects;
        Dictionary<string, Tag> _tags;
        List<string> _tagIDs;

        Dictionary<string, Anchor> _anchors;
        List<string> _anchorIDs;

        private string _host;
        private int _port;


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

    public class SimObject
    {
        private List<Tag> _tags;
        private PosData _posData;

        public SimObject()
        {
            _tags = new List<Tag>();
        }

    }

    public class Tag
    {
        List<PosData> _tagdata;
        private string _id;
        public Tag(string ID)
        {
            _id = ID;
            _tagdata = new List<PosData>();
        }

        public string getID() { return _id; }

        public PosData GetLatestPosData()
        {
            return _tagdata.First();
        }

        public void AddData(PosData data)
        {
            _tagdata.Add(data);
        }
    }

    /// <summary>
    /// Contains a set of position data
    /// </summary>
    public struct PosData
    {
        public float x, y, z;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ID"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_z"></param>
        public PosData(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }

}
