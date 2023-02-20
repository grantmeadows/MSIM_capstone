using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimulationEnviornment;
using Microsoft.VisualBasic;

namespace filereader
{

    /// <summary>
    /// Mqqt client for subscribing to pozyx broker
    /// </summary>
    public class Reader
    {
        private SimulationEnviornment.SimEnvironment _sim;
        private string _filename;
        private int time;

        /// <summary>
        /// Initializes and begins asynch subscription to tag topic from pozyx broker
        /// </summary>
        /// <param name="_numTags">Number of tags to be tracked</param>
        /// <param name="host">Host of the pozyx broker</param>
        /// <param name="port">Port</param>
        public Reader(string filename, SimulationEnviornment.SimEnvironment S)
        {
            _sim = S;
            _filename = filename;
            Task.Run(() => this.StartAsync());
            Thread.Sleep(Timeout.Infinite);
        }

        public async Task StartAsync()
        {
            int L = 0;
            string[] file = File.ReadAllLines(_filename);
            
            var msgObj = JArray.Parse(file[L]);
            var msgData = JArray.Parse(msgObj.ToString());

            time = msgData[0]["timestamp"].Value<int>();
            _sim.PushData(msgData);
            L++;
            msgObj = JArray.Parse(file[L]);
            msgData = JArray.Parse(msgObj.ToString());
            int next = msgData[0]["timestamp"].Value<int>();
            while (L < file.Length)
            {
                while(time > next)
                {
                    _sim.PushData(msgData);
                    L++;
                    msgObj = JArray.Parse(file[L]);
                    msgData = JArray.Parse(msgObj.ToString());
                    next = msgData[0]["timestamp"].Value<int>();

                    Dictionary<string, PosData> Pos = _sim.getAllPositions();
                    foreach (var ID in _sim.GetTagIDs())
                    {
                        Console.Write("[Tag ID: ");
                        Console.Write(ID);
                        Console.Write(": X: ");
                        Console.Write(Pos[ID].x);
                        Console.Write(" Y: ");
                        Console.Write(Pos[ID].y);
                        Console.Write(" Z: ");
                        Console.Write(Pos[ID].z);
                        Console.Write("] ");
                    }
                    Console.WriteLine(" ");

                }
                Thread.Sleep(1000);
                time += 1;

            }
        }
    }
}