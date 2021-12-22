using Rido.Mqtt.PnPApi;
using Rido.Mqtt.PnPApi.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsPnPClient
{
    public class UpdateShadowBinder : IPropertyStoreWriter
    {
        private readonly ServiceRequestResponseBinder binder;
        public UpdateShadowBinder(IMqttConnection c) => 
            binder = new ServiceRequestResponseBinder(
                c, 
                "$aws/things/{clientId}/shadow/update", 
                "$aws/things/{clientId}/shadow/update/accepted", "");
        
        public Task<string> ReportPropertyAsync(object payload, CancellationToken token = default) =>
            binder.SendRequestWaitForResponseAsync(payload, token);
        
    }
}
