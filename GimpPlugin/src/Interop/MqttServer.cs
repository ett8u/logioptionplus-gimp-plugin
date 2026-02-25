namespace Loupedeck.GimpPlugin.Interop
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Server;

    public class MqttServer
    {
        private static MqttServer _instance;
        private MqttServer _server;
        private const int Port = 1883;
        private const string Topic = "gimp/commands";
        private const string StateTopic = "gimp/state";

        public static MqttServer Instance => _instance ??= new MqttServer();

        public event EventHandler<string> StateReceived;

        private MqttServer() { }

        public async Task StartAsync()
        {
            var factory = new MqttFactory();
            var options = factory.CreateServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(Port)
                .Build();

            _server = factory.CreateMqttServer(options);

            _server.InterceptingPublishAsync += OnMessageReceived;

            await _server.StartAsync();
            PluginLog.Info($"MQTT Server started on port {Port}");
        }

        public async Task StopAsync()
        {
            if (_server != null)
            {
                await _server.StopAsync();
                PluginLog.Info("MQTT Server stopped");
            }
        }

        private Task OnMessageReceived(InterceptingPublishEventArgs args)
        {
            if (args.ApplicationMessage.Topic == StateTopic)
            {
                var payload = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
                StateReceived?.Invoke(this, payload);
            }
            return Task.CompletedTask;
        }

        public async Task PublishCommandAsync(string command, string parameters = "")
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(Topic)
                .WithPayload($"{{\"command\":\"{command}\",\"params\":\"{parameters}\"}}")
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _server.InjectApplicationMessage(new InjectedMqttApplicationMessage(message)
            {
                SenderClientId = "GimpPlugin"
            });
        }
    }
}
