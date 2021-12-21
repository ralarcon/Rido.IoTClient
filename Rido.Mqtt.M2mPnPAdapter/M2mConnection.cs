using Rido.Mqtt.PnPApi;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace Rido.Mqtt.M2mPnPAdapter
{
    public class M2mConnection : IMqttConnection
    {
        private readonly MqttClient client;

        public bool IsConnected => client.IsConnected;

        public string ClientId => client.ClientId;

        public event Func<MqttMessage, Task>? OnMessage;
        public event EventHandler<DisconnectEventArgs>? OnMqttClientDisconnected;

        public M2mConnection(string host, string clientId, string user, string password)
        {
            client = new MqttClient(host, 8883, true, MqttSslProtocols.TLSv1_2, null, null);

            client.Connect(clientId, user, password);
            Console.WriteLine("Connected");

            client.MqttMsgPublishReceived += (sender, e) => OnMessage?.Invoke(new MqttMessage() { Topic = e.Topic, Payload = Encoding.UTF8.GetString(e.Message) });
            client.ConnectionClosed += (sender, e) => OnMqttClientDisconnected?.Invoke(sender, new DisconnectEventArgs() { ReasonInfo = "m2m does not provide disconnect info" });
        }

        public async Task<int> PublishAsync(string topic, string payload, int qos = 0, CancellationToken token = default)
        {
            var res = client.Publish(topic, Encoding.UTF8.GetBytes(payload));
            Console.WriteLine($"-> {topic} {payload}");
            return await Task.FromResult(Convert.ToInt32(res));
        }

        public async Task<int> SubscribeAsync(string topic, CancellationToken token = default)
        {
            var res = client.Subscribe(new string[] { topic }, new byte[] { 0 });
            Console.WriteLine($"+ {topic}");
            return await Task.FromResult(Convert.ToInt32(res));
        }
    }
}