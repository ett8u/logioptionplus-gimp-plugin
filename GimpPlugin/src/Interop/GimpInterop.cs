namespace Loupedeck.GimpPlugin.Interop
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// GIMP integration via MQTT pub/sub
    /// </summary>
    public class GimpInterop
    {
        private static GimpInterop _instance;
        private bool _isConnected = false;
        private MqttBroker _broker;

        // MQTT Topics
        private const string TOPIC_GIMP_ACTION = "gimp/action";
        private const string TOPIC_GIMP_STATUS = "gimp/status";
        private const string TOPIC_MX_HAPTIC = "mx/haptic";
        private const string TOPIC_MX_ICON = "mx/icon";

        public static GimpInterop Instance => _instance ??= new GimpInterop();

        private GimpInterop()
        {
            _broker = MqttBroker.Instance;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                // Start MQTT broker
                await _broker.StartAsync();
                
                _isConnected = true;
                PluginLog.Info("GIMP Interop connected via MQTT");
                
                // Publish connection status
                await PublishStatusAsync("connected");
                
                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to connect GIMP Interop: {ex.Message}");
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_isConnected)
            {
                await PublishStatusAsync("disconnected");
                await _broker.StopAsync();
                _isConnected = false;
                PluginLog.Info("GIMP Interop disconnected");
            }
        }

        public bool IsConnected() => _isConnected;

        public async Task<bool> InvokeOperationAsync(string operationName, params object[] parameters)
        {
            if (!_isConnected)
            {
                PluginLog.Error($"Cannot invoke {operationName}: Not connected");
                return false;
            }

            try
            {
                PluginLog.Info($"Invoking GIMP operation: {operationName}");

                // Publish action to GIMP via MQTT
                var actionMessage = new
                {
                    operation = operationName,
                    parameters = parameters,
                    timestamp = DateTime.UtcNow.ToString("o")
                };

                var json = JsonSerializer.Serialize(actionMessage);
                await _broker.PublishAsync(TOPIC_GIMP_ACTION, json);

                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to invoke {operationName}: {ex.Message}");
                return false;
            }
        }

        public async Task SendHapticFeedbackAsync(string feedbackType, int intensity = 50)
        {
            if (!_isConnected) return;

            try
            {
                var hapticMessage = new
                {
                    type = feedbackType,
                    intensity = intensity,
                    timestamp = DateTime.UtcNow.ToString("o")
                };

                var json = JsonSerializer.Serialize(hapticMessage);
                await _broker.PublishAsync(TOPIC_MX_HAPTIC, json);

                PluginLog.Info($"Sent haptic feedback: {feedbackType}");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to send haptic feedback: {ex.Message}");
            }
        }

        public async Task UpdateIconAsync(string actionName, string iconData)
        {
            if (!_isConnected) return;

            try
            {
                var iconMessage = new
                {
                    action = actionName,
                    icon = iconData,
                    timestamp = DateTime.UtcNow.ToString("o")
                };

                var json = JsonSerializer.Serialize(iconMessage);
                await _broker.PublishAsync(TOPIC_MX_ICON, json);

                PluginLog.Info($"Updated icon for: {actionName}");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to update icon: {ex.Message}");
            }
        }

        private async Task PublishStatusAsync(string status)
        {
            var statusMessage = new
            {
                status = status,
                timestamp = DateTime.UtcNow.ToString("o")
            };

            var json = JsonSerializer.Serialize(statusMessage);
            await _broker.PublishAsync(TOPIC_GIMP_STATUS, json);
        }

        // Synchronous wrappers for backward compatibility
        public bool Connect()
        {
            return ConnectAsync().GetAwaiter().GetResult();
        }

        public void Disconnect()
        {
            DisconnectAsync().GetAwaiter().GetResult();
        }

        public bool InvokeOperation(string operationName, params object[] parameters)
        {
            return InvokeOperationAsync(operationName, parameters).GetAwaiter().GetResult();
        }

        public bool HasActiveImage() => _isConnected;
        public int GetActiveImageId() => _isConnected ? 1 : -1;
        public bool HasActiveSelection() => false;
    }
}
