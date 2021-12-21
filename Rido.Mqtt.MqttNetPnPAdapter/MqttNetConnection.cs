using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using Rido.Mqtt.PnPApi;
using System.Diagnostics;
using System.Text;

namespace Rido.Mqtt.MqttNetPnPAdapter
{
    public class MqttNetConnection : IMqttConnection
    {
        private readonly IMqttClient client;

        public bool IsConnected => client.IsConnected;

        public string ClientId => client.Options.ClientId;

        public event Func<MqttMessage, Task>? OnMessage;
        public event EventHandler<DisconnectEventArgs>? OnMqttClientDisconnected;

        public static async Task<IMqttConnection> CreateAsync(string host, string clientId, string username, string password)
        {
            var client = new MqttFactory().CreateMqttClient(MqttNetTraceLogger.CreateTraceLogger());
            await client.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer(host, 8883)
                .WithTls()
                .WithClientId(clientId)
                .WithCredentials(username, password)
                .Build());

            Console.WriteLine("Connected");
            return new MqttNetConnection(client);
        }

        private MqttNetConnection(IMqttClient client)
        {
            this.client = client;

            client.ApplicationMessageReceivedAsync += async m =>
            {
                ArgumentNullException.ThrowIfNull(OnMessage);
                await OnMessage.Invoke(
                    new MqttMessage()
                    {
                        Topic = m.ApplicationMessage.Topic,
                        Payload = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>())
                    });
            };

            client.DisconnectedAsync += async d =>
            {
                ArgumentNullException.ThrowIfNull(OnMqttClientDisconnected);
                OnMqttClientDisconnected.Invoke(client, new DisconnectEventArgs() { ReasonInfo = d.Reason.ToString() });
                await Task.Yield();
            };
        }

        public async Task<int> PublishAsync(string topic, string payload, int qos = 0, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            var res = await client.PublishAsync(new MqttApplicationMessage() { Topic = topic, Payload = Encoding.UTF8.GetBytes(payload) }, token);
            if (res.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                throw new ApplicationException("Error publishing to " + topic);
            }
            Console.WriteLine($"-> {topic} {payload}");
            return 0;
        }

        public async Task<int> SubscribeAsync(string topic, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            var res = await client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build(), token);
            var errs = res.Items.Any(x => x.ResultCode > MqttClientSubscribeResultCode.GrantedQoS2);
            if (errs)
            {
                throw new ApplicationException("Error subscribing to " + topic);
            }
            Console.WriteLine($"+ {topic}");
            return 0;

        }
    }

    public class MqttNetTraceLogger
    {
        public static MqttNetEventLogger CreateTraceLogger()
        {
            var logger = new MqttNetEventLogger();
            logger.LogMessagePublished += (s, e) =>
            {
                var trace = $">> [{e.LogMessage.Timestamp:O}] [{e.LogMessage.ThreadId}]: {e.LogMessage.Message}";
                if (e.LogMessage.Exception != null)
                {
                    trace += Environment.NewLine + e.LogMessage.Exception.ToString();
                }

                Trace.TraceInformation(trace);
            };
            return logger;
        }
    }
}