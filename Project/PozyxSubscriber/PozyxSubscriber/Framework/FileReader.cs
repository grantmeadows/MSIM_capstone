using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


namespace PozyxSubscriber.Framework
{

    /// <summary>
    /// Mqqt client for subscribing to pozyx broker
    /// </summary>
    public class Reader
    {
        private SimEnvironment _sim;
        private string _filename;
        private float time;

        /// <summary>
        /// Initializes and begins asynch subscription to tag topic from pozyx broker
        /// </summary>
        /// <param name="_numTags">Number of tags to be tracked</param>
        /// <param name="host">Host of the pozyx broker</param>
        /// <param name="port">Port</param>
        public Reader(string filename, SimEnvironment S)
        {
            _sim = S;
            _filename = filename;
            Task.Run(() => StartAsync());
            Thread.Sleep(Timeout.Infinite);
        }

        public async Task StartAsync()
        {
            int L = 0;
            string[] file = File.ReadAllLines(_filename);

            var msgObj = JArray.Parse(file[L]);
            var msgData = JArray.Parse(msgObj.ToString());

            string s = msgData[0]["timestamp"].Value<string>();
            s = s.Remove(0, 5);
            double d = Convert.ToDouble(s);
            time = (float)d * 1000;
            _sim.PushData(msgData);
            L++;
            msgObj = JArray.Parse(file[L]);
            msgData = JArray.Parse(msgObj.ToString());
            s = msgData[0]["timestamp"].Value<string>();
            s = s.Remove(0, 5);
            d = Convert.ToDouble(s);
            float next = (float)d * 1000;
            while (L < file.Length)
            {
                _sim.PushData(msgData);
                L++;
                msgObj = JArray.Parse(file[L]);
                msgData = JArray.Parse(msgObj.ToString());
                s = msgData[0]["timestamp"].Value<string>();
                s = s.Remove(0, 5);
                d = Convert.ToDouble(s);
                next = (float)d * 1000;


                Dictionary<string, Vector3D> Pos = _sim.getAllPositions();
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

                int sleep = (int)(next - time);
                if (sleep < 0) sleep = 0;
                time = next;
                Thread.Sleep(sleep);


            }
        }
    }
}