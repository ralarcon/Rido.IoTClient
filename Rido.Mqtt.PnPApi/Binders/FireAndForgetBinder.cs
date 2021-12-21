using Rido.Mqtt.PnPApi;

namespace Rido.Mqtt.PnPApi.Binders
{
    public class FireAndForgetBinder
    {
        private readonly IMqttConnection connection;
        private readonly string topic;
        public FireAndForgetBinder(IMqttConnection c, string t)
        {
            connection = c;
            topic = t.Replace("{clientId}", c.ClientId);
        }

        public Task<int> SendAsync(string payload, CancellationToken token = default) => connection.PublishAsync(topic, payload, 0, token);

    }
}
