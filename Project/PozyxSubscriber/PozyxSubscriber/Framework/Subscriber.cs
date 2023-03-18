using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace PozyxSubscriber.Framework
{

    /// <summary>
    /// Mqqt client for subscribing to pozyx broker
    /// </summary>
    public class MqttClient
    {
        private IMqttClient _mqttClient;
        private IMqttClientOptions _options;
        private string _topic;
        private SimEnvironment _sim;
        private StringBuilder log = new StringBuilder();

        private string _filename;

        /// <summary>
        /// Initializes and begins asynch subscription to tag topic from pozyx broker
        /// </summary>
        /// <param name="_numTags">Number of tags to be tracked</param>
        /// <param name="host">Host of the pozyx broker</param>
        /// <param name="port">Port</param>

        public MqttClient(int _numTags, string host, int port, SimEnvironment Sim)
        {
            _sim = Sim;

            _topic = "tags";
        } 
        public MqttClient(int _numTags, string host, int port, SimEnvironment Sim, string filename)
        {
            _sim = Sim;
            _filename = filename;
            this._topic = "tags";


            _options = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .Build();

            _mqttClient = new MqttFactory().CreateMqttClient();
            _mqttClient.UseConnectedHandler(ConnectedHandler);
            _mqttClient.UseApplicationMessageReceivedHandler(MessageHandler);
            _mqttClient.UseDisconnectedHandler(DisconnectedHandler);

            //Comment out sleep before finalization
        }

        public void Begin()
        {
            Task.Run(() => StartAsync());
        }

        public async void ConnectedHandler(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("Connected with server!");
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_topic).Build());
            Console.WriteLine("Subscribed to topic");

            _sim.ConnectedStatus = true;
        }

        public void DisconnectedHandler(MqttClientDisconnectedEventArgs eventArgs)
        {
            Console.WriteLine("Disconnected from server");
            _sim.ConnectedStatus = false;
        }


        public void MessageHandler(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            //Console.WriteLine(Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload));

            var msg = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);

            var msgData = JArray.Parse(msg);
            var msgObj = JArray.Parse(msgData.ToString());

            _sim.PushData(msgObj);

            log.AppendLine(msg.ToString());
            File.WriteAllText(_filename, log.ToString());

            //foreach (var ID in _sim.TagIDs)
            //{
            //    Vector3D pos = _sim.GetTag(ID).Position;
            //    Console.Write("[Tag ID: ");
            //    Console.Write(ID);
            //    Console.Write(": X: ");
            //    Console.Write(pos.x);
            //    Console.Write("  Y: ");
            //    Console.Write(pos.y);
            //    Console.Write("  Z: ");
            //    Console.Write(pos.z);
            //    Console.Write("] ");
            //}
            //Console.WriteLine(" ");

        }

        public async Task StartAsync()
        {
            await _mqttClient.ConnectAsync(_options, CancellationToken.None);
        }
    }
}