﻿using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class DesiredUpdatePropertyBinder<T>
    {
        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated = null;
        public DesiredUpdatePropertyBinder(IMqttClient connection, string propertyName, string componentName = "")
        {
            _ = connection.SubscribeAsync("$iothub/twin/PATCH/properties/desired/#");
            UpdateTwinBinder updateTwin = UpdateTwinBinder.GetInstance(connection);
            connection.ApplicationMessageReceivedAsync += async m =>
             {
                 var topic = m.ApplicationMessage.Topic;
                 if (topic.StartsWith("$iothub/twin/PATCH/properties/desired"))
                 {
                     string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                     JsonNode desired = JsonNode.Parse(msg);
                     JsonNode desiredProperty = TwinParser.ReadPropertyFromDesired(desired, propertyName, componentName);
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
                                 _ = updateTwin.ReportPropertyAsync(ack.ToAckDict());
                             }
                         }
                     }
                 }
             };
        }


    }
}
