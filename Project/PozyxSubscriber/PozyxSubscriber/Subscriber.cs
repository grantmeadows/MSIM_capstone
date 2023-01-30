using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimulationEnviornment;

namespace mqtt_c
{


    class MqttClient
    {
        private IMqttClient _mqttClient;
        private IMqttClientOptions _options;
        private string _topic;

        public MqttClient(int _numTags)
        {
            var host = "192.168.1.27";
            var port = 1883;
            this._topic = "tags";

            this._options = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .Build();

            this._mqttClient = new MqttFactory().CreateMqttClient();
            this._mqttClient.UseConnectedHandler(ConnectedHandler);
            this._mqttClient.UseApplicationMessageReceivedHandler(MessageHandler);
            this._mqttClient.UseDisconnectedHandler(DisconnectedHandler);

            Task.Run(() => this.StartAsync());
            Thread.Sleep(Timeout.Infinite);  
        }

        public async void ConnectedHandler(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("Connected with server!");
            await this._mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(this._topic).Build());
            Console.WriteLine("Subscribed to topic");
        }

        public void DisconnectedHandler(MqttClientDisconnectedEventArgs eventArgs)
        {
            Console.WriteLine("Disconnected from server");
        }


        public void MessageHandler(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            Console.WriteLine(Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload));

            //do parse, put 
        }

        public async Task StartAsync()
        {
            await this._mqttClient.ConnectAsync(this._options, CancellationToken.None);
        }
    }
}