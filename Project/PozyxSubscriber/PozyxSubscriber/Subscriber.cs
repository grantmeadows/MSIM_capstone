using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mqtt_c
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "192.168.1.27";
            var port = 1883;
            var topic = "tags";

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .Build();

            var mqttClient = new MqttClient(options, topic);
            Task.Run(() => mqttClient.StartAsync());
            Thread.Sleep(Timeout.Infinite);
        }
    }

    class MqttClient
    {
        private IMqttClient mqttClient;
        private IMqttClientOptions options;
        private string topic;

        public MqttClient(IMqttClientOptions options, String topic)
        {
            this.options = options;
            this.topic = topic;
            this.mqttClient = new MqttFactory().CreateMqttClient();
            this.mqttClient.UseConnectedHandler(ConnectedHandler);
            this.mqttClient.UseApplicationMessageReceivedHandler(MessageHandler);
            this.mqttClient.UseDisconnectedHandler(DisconnectedHandler);
        }

        public async void ConnectedHandler(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("Connected with server!");
            await this.mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(this.topic).Build());
            Console.WriteLine("Subscribed to topic");
        }

        public void DisconnectedHandler(MqttClientDisconnectedEventArgs eventArgs)
        {
            Console.WriteLine("Disconnected from server");
        }

        public void MessageHandler(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            Console.WriteLine(Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload));
        }

        public async Task StartAsync()
        {
            await this.mqttClient.ConnectAsync(this.options, CancellationToken.None);
        }
    }
}