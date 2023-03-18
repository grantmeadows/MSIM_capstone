using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.IO;
using System.Collections.Generic;
using MQTTnet.Client.Publishing;

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
        }

        public void Begin()
        {
            Task.Run(() => StartAsync());
        }

        public async Task StartAsync()
        {
            int L = 0;
            string[] file = File.ReadAllLines(_filename);

            var msgObj = JArray.Parse(file[L]);
            var msgData = JArray.Parse(msgObj.ToString());
            _sim.ConnectedStatus = true;
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


                //foreach (var ID in _sim.TagIDs)
                //{
                //    Vector3D pos = _sim.GetTag(ID).Position;
                //    Console.Write("[Tag ID: ");
                //    Console.Write(ID);
                //    Console.Write(": X: ");
                //    Console.Write(pos.x);
                //    Console.Write(" Y: ");
                //    Console.Write(pos.y);
                //    Console.Write(" Z: ");
                //    Console.Write(pos.z);
                //    Console.Write("] ");
                //}
                //Console.WriteLine(" ");

                int sleep = (int)(next - time);
                if (sleep < 0) sleep = 0;
                time = next;
                Thread.Sleep(sleep);


            }
            _sim.ConnectedStatus = false;
        }
    }
}