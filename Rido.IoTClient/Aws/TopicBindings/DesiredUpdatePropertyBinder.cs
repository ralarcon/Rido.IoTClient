﻿using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws.TopicBindings
{
    public class DesiredUpdatePropertyBinder<T>
    {
        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated = null;
        public DesiredUpdatePropertyBinder(IMqttClient connection, string deviceId, string propertyName, string componentName = "")
        {
            _ = connection.SubscribeAsync($"$aws/things/{deviceId}/shadow/update");
            //UpdateTwinBinder updateTwin = new UpdateTwinBinder(connection);
            connection.ApplicationMessageReceivedAsync += async m =>
             {
                 var topic = m.ApplicationMessage.Topic;
                 if (topic.StartsWith($"$aws/things/{deviceId}/shadow/update"))
                 {
                     string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                     JsonNode desired = JsonNode.Parse(msg);
                     JsonNode desiredProperty = null;
                     if (string.IsNullOrEmpty(componentName))
                     {
                         desiredProperty = desired?[propertyName];
                     }
                     else
                     {
                         if (desired[componentName] != null &&
                             desired[componentName][propertyName] != null &&
                             desired[componentName]["__t"] != null &&
                             desired[componentName]["__t"].GetValue<string>() == "c")

                             desiredProperty = desired?[componentName][propertyName];
                     }

                     if (desiredProperty != null)
                     {
                         if (OnProperty_Updated != null)
                         {
                             var property = new PropertyAck<T>(propertyName, componentName)
                             {
                                 Value = desiredProperty.GetValue<T>(),
                                 Version = desired?["$version"]?.GetValue<int>() ?? 0
                             };
                             var ack = await OnProperty_Updated(property);
                             if (ack != null)
                             {
                                // _ = updateTwin.UpdateTwinAsync(ack.ToAck());
                             }
                         }
                     }
                 }
             };
        }
    }
}