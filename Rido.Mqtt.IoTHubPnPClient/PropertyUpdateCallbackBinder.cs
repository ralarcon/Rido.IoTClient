using Rido.Mqtt.PnPApi;
using Rido.Mqtt.PnPApi.Binders;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Rido.Mqtt.IoTHubPnPClient
{
    public class PropertyUpdateCallbackBinder<T> : IPropertyUpdateCallback<T>
    {
        private readonly string propertyName;
        private readonly string componentName;

        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated { get; set; }

        public PropertyUpdateCallbackBinder(IMqttConnection connection, string propertyName, string componentName = "")
        {
            this.propertyName = propertyName;
            this.componentName = componentName;
            var binder = new RequestResponseBinder(
                connection,
                "$iothub/twin/PATCH/properties/desired/",
                "$iothub/twin/PATCH/properties/reported/")
            {
                RequestHandler = ProcessPropertyUpdate
            };
        }

        private async Task<string> ProcessPropertyUpdate(string propertyJson)
        {
            var ack = new PropertyAck<T>(propertyName);
            var desired = JsonNode.Parse(propertyJson);
            var desiredProperty = TwinParser.ReadPropertyFromDesired(desired, propertyName, componentName);
            if (desiredProperty != null)
            {
                if (OnProperty_Updated != null)
                {
                    var property = new PropertyAck<T>(propertyName, componentName)
                    {
                        Value = desiredProperty.Deserialize<T>(),
                        Version = desired?["$version"]?.GetValue<int>() ?? 0
                    };
                    ack = await OnProperty_Updated(property);
                }
            }
            return JsonSerializer.Serialize(ack.ToAckDict());
        }
    }
}
