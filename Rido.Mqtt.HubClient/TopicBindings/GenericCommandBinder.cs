﻿
using Rido.MqttCore;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient.TopicBindings
{
    public class GenericCommand
    {
        public Func<GenericCommandRequest, Task<GenericCommandResponse>> OnCmdDelegate { get; set; }

        public GenericCommand(IMqttBaseClient connection)
        {
            _ = connection.SubscribeAsync("$iothub/methods/POST/#");
            connection.OnMessage += async m =>
            {
                var topic = m.Topic;
                if (topic.StartsWith($"$iothub/methods/POST/"))
                {
                    var segments = topic.Split('/');
                    var cmdName = segments[3];
                    string msg = m.Payload;
                    GenericCommandRequest req = new GenericCommandRequest()
                    {
                        CommandName = cmdName,
                        CommandPayload = msg
                    };
                    if (OnCmdDelegate != null && req != null)
                    {
                        (int rid, _) = TopicParser.ParseTopic(topic);
                        GenericCommandResponse response = await OnCmdDelegate.Invoke(req);
                        _ = connection.PublishAsync($"$iothub/methods/res/{response.Status}/?$rid={rid}", JsonSerializer.Serialize(response));
                    }
                }
            };
        }
    }
}
