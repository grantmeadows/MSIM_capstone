using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mqtt_c;

namespace SimulationEnviornment
{

    /// <summary>
    /// Simulation enviornemnt
    /// </summary>
    public class SimEnviornment
    {
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
            _MqqtClient = new MqttClient(numTags, host, port);
            _host = host;
            _port = port;

        }

               

        private static MqttClient _MqqtClient;
        List<SimObject> _objects;
        private string _host;
        private int _port;


    }


    public class SimObject
    { 
        private PosData _posData;

    }

    /// <summary>
    /// Contains a set of position data
    /// </summary>
    public struct PosData
    {
        public string ID;
        public float x, y, z;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_ID"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_z"></param>
        public PosData(string _ID, float _x, float _y, float _z)
        {
            ID = _ID;
            x = _x;
            y = _y;
            z = _z;
        }
    }

}
