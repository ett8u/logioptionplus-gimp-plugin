namespace Loupedeck.GimpPlugin.Interop
{
    using System;
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Protocol;
    using MQTTnet.Server;

    public class MqttBroker
    {
        private static MqttBroker _instance;
        private MqttServer _server;
        private bool _isRunning = false;

        public static MqttBroker Instance => _instance ??= new MqttBroker();

        private MqttBroker() { }

        public async Task StartAsync()
        {
            if (_isRunning)
            {
                PluginLog.Info("MQTT Broker already running");
                return;
            }

            try
            {
                var factory = new MqttFactory();
                
                var options = new MqttServerOptionsBuilder()
                    .WithDefaultEndpoint()
                    .WithDefaultEndpointPort(1883)
                    .Build();

                _server = factory.CreateMqttServer(options);

                _server.ClientConnectedAsync += e =>
                {
                    PluginLog.Info($"MQTT Client connected: {e.ClientId}");
                    return Task.CompletedTask;
                };

                _server.InterceptingPublishAsync += e =>
                {
                    var payload = e.ApplicationMessage.ConvertPayloadToString();
                    PluginLog.Info($"MQTT: {e.ApplicationMessage.Topic} = {payload}");
                    return Task.CompletedTask;
                };

                await _server.StartAsync();
                _isRunning = true;

                PluginLog.Info("MQTT Broker started on port 1883");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to start MQTT Broker: {ex.Message}");
                throw;
            }
        }

        public async Task StopAsync()
        {
            if (!_isRunning || _server == null) return;

            try
            {
                await _server.StopAsync();
                _isRunning = false;
                PluginLog.Info("MQTT Broker stopped");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error stopping MQTT Broker: {ex.Message}");
            }
        }

        public bool IsRunning() => _isRunning;

        public async Task PublishAsync(string topic, string payload)
        {
            if (!_isRunning || _server == null)
            {
                PluginLog.Warning("MQTT Broker not running");
                return;
            }

            try
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await _server.InjectApplicationMessage(
                    new InjectedMqttApplicationMessage(message)
                    {
                        SenderClientId = "GimpPlugin"
                    });

                PluginLog.Info($"Published: {topic}");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Publish failed: {ex.Message}");
            }
        }
    }
}
