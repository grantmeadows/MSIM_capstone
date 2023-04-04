using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Timers;

namespace PozyxPositioner.Framework
{

    /// <summary>
    /// Simulation enviornemnt
    /// </summary>
    public sealed class SimEnvironment
    {

        private static SimEnvironment _instance = null;
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
        /// Simulation Enviorntment Constructor, creates MqqtClient instance, includes a filename to log all messages
        /// starting subscription to tags topic
        /// </summary>
        /// <param name="host">Local IP addres of Pozyx gateway</param>
        /// <param name="port">Port</param>
        /// <param name="filename">a log file to keep track of all tag messages</param>
        /// <param name="refreshRate">Default refresh rate of a tag not specified by the user</param>
        public void Initialize(string host, int port, string filename, int refreshRate)
        {
            reader = false;
            _host = host;
            _port = port;
            _tags = new Dictionary<string, Tag>();
            _tagIDs = new List<string>();
            _refreshRate = refreshRate;
            _MqqtClient = new MqttClient(host, port, this, filename);
        }


        /// <summary>
        /// Simulation Enviorntment Constructor, creates MqqtClient instance
        /// starting subscription to tags topic
        /// </summary>
        /// <param name="host">Local IP addres of Pozyx gateway</param>
        /// <param name="port">Port</param>
        /// <param name="refreshRate">Default refresh rate of a tag not specified by the user</param>
        public void Initialize(string host, int port, int refreshRate)
        {
            reader = false;
            _host = host;
            _port = port;
            _tags = new Dictionary<string, Tag>();
            _tagIDs = new List<string>();
            _refreshRate = refreshRate;
            _MqqtClient = new MqttClient(host, port, this);
        }


        /// <summary>
        /// Simulation Enviorntment Constructor, creates reader instance
        /// will send JSON strings in coordination to how POZYX does using a log file.
        /// Simulates reciving tag reads in realtime using a historic log
        /// </summary>
        /// <param name="filename">name of the log file to simulate with</param>
        /// <param name="refreshRate">Default refresh rate of a tag not specified by the user</param>
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
            _filename = filename;
        }

        /// <summary>
        /// StartEnvironment method, will begin tracking/reading JSON strings and tag readings from Pozyx on a separate thread
        /// This begins populating the simulation environment
        /// </summary>
        public void StartEnvironment()
        {

            if (reader) Task.Run(() => StartAsyncReader()); 
            else _MqqtClient.Begin();
        }


        private static MqttClient _MqqtClient;
        Dictionary<string, Tag> _tags;
        List<string> _tagIDs;
        string _filename;
        private string _host;
        private int _port;

        int _refreshRate;

        /// <summary>
        /// The Connection status of the MQTT or Reader class
        /// </summary>
        public bool ConnectedStatus
        {
            get { return _connectedStatus; }
            set { _connectedStatus = value; }
        }


        /// <summary>
        /// Pushes a JSON JArray into the simulation
        /// </summary>
        /// <param name="msgdata"> the JArray parsed to populate the simulation environment with</param>
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
                    try
                    {
                        z = M["data"]["coordinates"]["z"].Value<float>();
                    }
                    catch { z = 0; }
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
                    newData.Acceleration = new List<PozyxVector>();
                    for (int i = 0; i < Accel.Count(); i++)
                    {
                        PozyxVector vect = new PozyxVector();
                        vect.x = temp[0].Value<float>();
                        vect.y = temp[1].Value<float>();
                        try { vect.z = temp[2].Value<float>(); }
                        catch { vect.z = 0; }

                        newData.Acceleration.Add(vect);
                        temp = temp.Next;
                    }
                }
                else
                {
                    newData.Acceleration = new List<PozyxVector>();
                    newData.Acceleration.Add(new PozyxVector());
                }


                if (_tags.ContainsKey(ID))
                    _tags[ID].AddData(newData);
                else
                {
                    //_tagIDs.Add(ID);
                    _tags[ID] = new Tag(ID, _refreshRate);
                    //_tags[ID].AddData(newData);
                }




            }
        }


        /// <summary>
        /// Adds a new tag to the simulation environment
        /// </summary>
        /// <param name="ID"> the ID of the tag to add</param>
        /// <param name="refreshRate"> the refresh rate of the added tag</param>
        /// <returns> the newly created tag </returns>
        public Tag newTag(string ID, int refreshRate)
        {
            if (_tags.ContainsKey(ID))
            {
                throw new Exception("tag declared twice within Pozyx Environment");
            }
            //_tagIDs.Add(ID);
            _tags[ID] = new Tag(ID, refreshRate);
            //_tags[ID].AddData(new PosData());
            return _tags[ID];
        }


        /// <summary>
        /// Adds a new tag to the simulation environment
        /// </summary>
        /// <param name="tag"> the tag to add</param>
        public void newTag(Tag tag)
        {
            string ID = tag.ID;
            if (_tags.ContainsKey(ID))
            {
                throw new Exception("tag declared twice within Pozyx Environment");
            }
            _tagIDs.Add(ID);
            _tags[ID] = tag;
            _tags[ID].AddData(new PosData());
        }


        /// <summary>
        /// Removes a tag from the simulation environment
        /// </summary>
        /// <param name="ID"> the ID of the tag to remove</param>
        /// <returns> the removed tag </returns>
        public Tag RemoveTag(string ID)
        {
            Tag T = _tags[ID];
            _tagIDs.Remove(ID);
            _tags.Remove(ID);
            return T;
        }

        //public float GetDistanceBetweenTags(string t1, string t2)
        //{
        //    PozyxVector t1Pos = _tags[t1].Position;
        //    //PozyxVector t2Pos = _tags[t2].Position;

        //    float lhs = (float)MathF.Pow(t2Pos.x - t1Pos.x, 2);
        //    float rhs = (float)MathF.Pow(t2Pos.y - t1Pos.y, 2);

        //    return MathF.Sqrt(lhs + rhs);
        //}


        /// <summary>
        /// gets a tag from the simulation environment
        /// </summary>
        /// <param name="ID"> the ID of the tag to get</param>
        /// <returns> the requested tag </returns>
        public Tag GetTag(string ID) { return _tags[ID]; }

        /// <summary>
        /// a list of strings that contain the IDs of all the active tags in the Simulation Environment
        /// </summary>
        public List<string> TagIDs { get { return _tagIDs; } }



        private async Task StartAsyncReader()
        {

            float time;
            int L = 0;
            string[] file = File.ReadAllLines(_filename);

            var msgObj = JArray.Parse(file[L]);
            var msgData = JArray.Parse(msgObj.ToString());
            this.ConnectedStatus = true;
            string s = msgData[0]["timestamp"].Value<string>();
            s = s.Remove(0, 5);
            double d = Convert.ToDouble(s);
            time = (float)d * 1000;
            this.PushData(msgData);
            L++;
            msgObj = JArray.Parse(file[L]);
            msgData = JArray.Parse(msgObj.ToString());
            s = msgData[0]["timestamp"].Value<string>();
            s = s.Remove(0, 5);
            d = Convert.ToDouble(s);
            float next = (float)d * 1000;
            while (L < file.Length)
            {
                this.PushData(msgData);
                L++;
                msgObj = JArray.Parse(file[L]);
                msgData = JArray.Parse(msgObj.ToString());
                s = msgData[0]["timestamp"].Value<string>();
                s = s.Remove(0, 5);
                d = Convert.ToDouble(s);
                next = (float)d * 1000;

                int sleep = (int)(next - time);
                if (sleep < 0) sleep = 0;
                time = next;
                Thread.Sleep(sleep);


            }
            this.ConnectedStatus = false;
        }

    }


}
