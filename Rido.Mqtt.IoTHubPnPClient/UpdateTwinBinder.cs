using Rido.Mqtt.PnPApi;
using Rido.Mqtt.PnPApi.Binders;

namespace Rido.Mqtt.IoTHubPnPClient
{
    internal class UpdateTwinBinder : IPropertyStoreWriter<string>
    {
        private readonly ServiceRequestResponseBinder binder;

        public UpdateTwinBinder(IMqttConnection c) =>
            binder = new ServiceRequestResponseBinder(c, "$iothub/twin/PATCH/properties/reported/", "$iothub/twin/res/", 204);

        public Task<string> ReportPropertyAsync(object payload, CancellationToken token = default) =>
            binder.SendRequestWaitForResponseAsync(payload, token);
    }
}
