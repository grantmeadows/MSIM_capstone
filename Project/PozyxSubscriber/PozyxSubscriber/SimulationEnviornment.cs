using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mqtt_c;

namespace SimulationEnviornment
{



    public class SimulationEnviornment
    {
        public SimulationEnviornment() 
        {
        }
        public SimulationEnviornment(string host, int port, int numObjects, int numTags)
        {
            _MqqtClient = new MqttClient(numTags, host);
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


    public struct PosData
    {
        public string ID;
        public float x, y, z;

        public PosData(string _ID, float _x, float _y, float _z)
        {
            ID = _ID;
            x = _x;
            y = _y;
            z = _z;
        }
    }

}
