using Rido.Mqtt.PnPApi;
using System.Web;

namespace Rido.Mqtt.PnPApi.Binders
{
    public class RequestResponseBinder
    {
        private readonly string requestTopic;
        private readonly string responseTopic;
        private readonly string subFilter;
        private readonly IMqttConnection connection;
        public Func<string, Task<string>> RequestHandler;

        public RequestResponseBinder(IMqttConnection c, string reqTopic, string respTopic, string subFilter = "")
        {
            connection = c;
            requestTopic = reqTopic.Replace("{clientId}", c.ClientId);
            responseTopic = respTopic.Replace("{clientId}", c.ClientId);
            this.subFilter = subFilter;
            _ = c.SubscribeAsync(requestTopic + "#");
            c.OnMessage += C_OnMessage;
        }

        private async Task C_OnMessage(MqttMessage arg)
        {
            if (RequestHandler != null && arg.Topic.StartsWith(requestTopic + subFilter))
            {
                Console.WriteLine($"<- {arg.Topic}");

                string currentResponseTopic = responseTopic;
                if (GetRidFromTopic(arg.Topic, out int rid))
                {
                    currentResponseTopic = responseTopic.Replace("{rid}", rid.ToString());
                }

                string resp = await RequestHandler.Invoke(arg.Payload);

                if (resp != null)
                {
                    var puback = await connection.PublishAsync(currentResponseTopic, resp);

                    if (puback != -1)
                    {
                        throw new ApplicationException("error publishing cmd response");
                    }
                }
            }
        }

        private static bool GetRidFromTopic(string topic, out int rid)
        {
            bool result = false;
            var segments = topic.Split('/');
            rid = -1;

            if (topic.Contains('?'))
            {
                var qs = HttpUtility.ParseQueryString(segments[^1]);
                result = int.TryParse(qs["$rid"], out int r);
                if (result)
                {
                    rid = r;
                }
            }
            return result;
        }
    }
}