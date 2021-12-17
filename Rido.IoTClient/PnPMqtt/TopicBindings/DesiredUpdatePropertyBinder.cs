﻿using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Rido.IoTClient.PnPMqtt.TopicBindings
{
    public class DesiredUpdatePropertyBinder<T>
    {
        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated = null;
        public DesiredUpdatePropertyBinder(IMqttClient connection, string propertyName, string componentName = "")
        {
            _ = connection.SubscribeAsync($"pnp/{connection.Options.ClientId}/props/set/#");
            UpdatePropertyBinder propertyBinder = UpdatePropertyBinder.GetInstance(connection);
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;
                if (topic.StartsWith($"pnp/{connection.Options.ClientId}/props/set"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    JsonNode desired = JsonNode.Parse(msg);
                    var desiredProperty = desired?[propertyName];
                    if (desiredProperty != null)
                    {
                        if (OnProperty_Updated != null)
                        {
                            var property = new PropertyAck<T>(propertyName, componentName)
                            {
                                Value = desiredProperty.Deserialize<T>(),
                                //Version = desired?["$version"]?.GetValue<int>() ?? 0
                            };
                            var ack = await OnProperty_Updated(property);
                            if (ack != null)
                            {
                                //_ = updateTwin.SendRequestWaitForResponse(ack);
                                _ = propertyBinder.ReportPropertyAsync(ack.ToAckDict());
                            }
                        }
                    }
                }
            };
        }
    }
}