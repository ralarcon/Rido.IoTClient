using Rido.Mqtt.PnPApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.PnPApi.Tests
{
    internal class MockMqttConnection : IMqttConnection
    {
        public bool IsConnected => throw new NotImplementedException();

        public string ClientId => throw new NotImplementedException();

        public event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        public event Func<MqttMessage, Task> OnMessage;

        public Task<int> PublishAsync(string topic, string payload, int qos = 0, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> SubscribeAsync(string topic, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}