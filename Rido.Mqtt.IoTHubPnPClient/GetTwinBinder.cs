using Rido.Mqtt.PnPApi;
using Rido.Mqtt.PnPApi.Binders;

namespace Rido.Mqtt.IoTHubPnPClient
{
    public class GetTwinBinder : IPropertyStoreReader
    {
        private readonly ServiceRequestResponseBinder binder;

        public GetTwinBinder(IMqttConnection c) =>
            binder = new ServiceRequestResponseBinder(c, "$iothub/twin/GET/", "$iothub/twin/res/", 200);

        public Task<string> ReadPropertiesDocAsync(CancellationToken token = default)
            => binder.SendRequestWaitForResponseAsync(string.Empty, token);

    }
}
