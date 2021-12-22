using Rido.Mqtt.PnPApi;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Rido.Mqtt.PnPApi.Binders
{
    public class ServiceRequestResponseBinder
    {
        private static int counter = 0;
        private static readonly ConcurrentDictionary<int, TaskCompletionSource<string>> pendingRequests = new();
        private readonly string requestTopic;
        private readonly string responseTopic;
        private readonly IMqttConnection connection;

        public ServiceRequestResponseBinder(IMqttConnection c, string reqTopic, string respTopic, string subFilter)
        {
            connection = c;
            requestTopic = reqTopic.Replace("{clientId}", c.ClientId);
            responseTopic = respTopic.Replace("{clientId}", c.ClientId);
            _ = connection.SubscribeAsync(responseTopic + "#");

            connection.OnMessage += async m =>
            {
                if (m.Topic.StartsWith(responseTopic + subFilter))
                {
                    if (pendingRequests.TryRemove(counter, out var tcs))
                    {
                        tcs.SetResult(m.Payload);
                    }
                }
                await Task.Yield();
            };
        }

        public async Task<string> SendRequestWaitForResponseAsync(object payload, CancellationToken token = default)
        {
            string jsonPayload;

            if (payload is string)
            {
                jsonPayload = payload as string ?? "";
            }
            else
            {
                jsonPayload = JsonSerializer.Serialize(payload);
            }

            counter++;
            var tcs = new TaskCompletionSource<string>();
            var puback = await connection.PublishAsync(requestTopic + "?$rid=" + counter, jsonPayload, 0, token);
            if (puback >= 0)
            {
                pendingRequests.TryAdd(counter, tcs);
            }
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
        }
    }
}
