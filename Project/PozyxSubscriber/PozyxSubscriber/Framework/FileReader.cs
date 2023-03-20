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

                int sleep = (int)(next - time);
                if (sleep < 0) sleep = 0;
                time = next;
                Thread.Sleep(sleep);


            }
            _sim.ConnectedStatus = false;
        }
    }
}