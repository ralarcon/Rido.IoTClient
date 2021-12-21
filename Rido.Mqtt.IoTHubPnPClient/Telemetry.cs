using Rido.Mqtt.PnPApi;
using Rido.Mqtt.PnPApi.Binders;
using System.Text.Json;

namespace Rido.Mqtt.IoTHubPnPClient
{
    public class Telemetry<T>
    {
        //private readonly IMqttConnection connection;
        //private readonly string deviceId;
        //private readonly string moduleId;
        //private readonly string componentName;
        private readonly string name;
        private readonly FireAndForgetBinder binder;

        public Telemetry(IMqttConnection connection, string name) //, string componentName = "", string moduleId = "")
        {
            //this.connection = connection;
            this.name = name;
            //this.componentName = componentName;
            //deviceId = connection.ClientId;
            //this.moduleId = moduleId;
            binder = new FireAndForgetBinder(connection, "devices/{clientId}/messages/events/");
        }

        public async Task<int> SendTelemetryAsync(T payload, CancellationToken cancellationToken = default)
        {
            //string topic = $"devices/{deviceId}";

            //if (!string.IsNullOrEmpty(moduleId))
            //{
            //    topic += $"/modules/{moduleId}";
            //}
            //topic += "/messages/events/";

            //if (!string.IsNullOrEmpty(componentName))
            //{
            //    topic += $"$.sub={componentName}";
            //}

            Dictionary<string, T> typedPayload = new()
            {
                { name, payload }
            };
            return await binder.SendAsync(JsonSerializer.Serialize(typedPayload), cancellationToken);
        }
    }
}
