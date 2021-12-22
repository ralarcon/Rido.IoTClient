using Rido.Mqtt.PnPApi;
using Rido.Mqtt.PnPApi.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsPnPClient
{
    public class GetShadowBinder : IPropertyStoreReader
    {
        private readonly ServiceRequestResponseBinder binder;

        public GetShadowBinder(IMqttConnection c) =>
            binder = new ServiceRequestResponseBinder(c, "$aws/things/{clientId}/shadow/get", "$aws/things/{clientId}/shadow/get/accepted", "");

        public Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default) =>
            binder.SendRequestWaitForResponseAsync(string.Empty, cancellationToken);
    }
}
