using Rido.Mqtt.PnPApi;
using Rido.Mqtt.PnPApi.Binders;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Rido.Mqtt.IoTHubPnPClient
{
    public class TwinUpdateCallbackBinder : IPropertyUpdateCallback
    {
        private readonly string propertyName;
        private readonly string componentName;

        public Func<string, Task<string>> OnProperty_Updated { get; set; }

        public TwinUpdateCallbackBinder(IMqttConnection connection, string propertyName, string componentName = "")
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
            //var ack = new PropertyAck<T>(propertyName);
            string ack = string.Empty;
            var desired = JsonNode.Parse(propertyJson);
            var desiredProperty = TwinParser.ReadPropertyFromDesired(desired, propertyName, componentName);
            if (desiredProperty != null)
            {
                if (OnProperty_Updated != null)
                {
                    //var property = new PropertyAck<object>(propertyName, componentName)
                    //{
                    //    Value = desiredProperty.Deserialize<T>(),
                    //    Version = desired?["$version"]?.GetValue<int>() ?? 0
                    //};
                    ack = await OnProperty_Updated(desiredProperty.ToJsonString());
                }
            }
            //return JsonSerializer.Serialize(ack.ToAckDict());
            return ack;
        }
    }
}
