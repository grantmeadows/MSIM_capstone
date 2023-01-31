using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationEnviornment;
using mqtt_c;

namespace PozyxSubscriber
{
    internal class Program
    {
        static public void Main(string[] args)
        {
            var host = "10.0.0.254";
            int numTags = 1;

            mqtt_c.MqttClient MQ = new mqtt_c.MqttClient(numTags, host);

        }


    }
}
