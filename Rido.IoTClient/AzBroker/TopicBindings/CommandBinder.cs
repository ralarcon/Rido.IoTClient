﻿
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using Rido.IoTClient;

namespace Rido.IoTClient.AzBroker.TopicBindings
{

    public class Command<T, TResponse>
        where T : IBaseCommandRequest<T>, new()
        where TResponse : BaseCommandResponse
    {
        public Func<T, Task<TResponse>> OnCmdDelegate { get; set; }

        public Command(IMqttClient connection, string commandName, string componentName = "")
        {
            _ = connection.SingleSubscribe("$az/iot/methods/+/+");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;

                var fullCommandName = string.IsNullOrEmpty(componentName) ? commandName : $"{componentName}*{commandName}";

                if (topic.StartsWith($"$az/iot/methods/{fullCommandName}"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    T req = new T().DeserializeBody(msg);
                    if (OnCmdDelegate != null && req != null)
                    {
                        (int rid, _) = TopicParser.ParseTopic(topic);
                        TResponse response = await OnCmdDelegate.Invoke(req);
                        _ = connection.PublishAsync($"$az/iot/methods/{fullCommandName}/response/?rid={rid}&rc={response.Status}", response);
                    }
                }
            };
        }
    }
}
