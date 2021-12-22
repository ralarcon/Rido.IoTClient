using Rido.Mqtt.PnPApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rido.Mqtt.AwsPnPClient
{
    public class BaseAwsPnPClient
    {
        public readonly IMqttConnection Connection;

        public ConnectionSettings ConnectionSettings;
        public string InitialState = string.Empty;

        protected readonly IPropertyStoreReader getShadowBinder;
        protected readonly IPropertyStoreWriter updateShadowBinder;

        public BaseAwsPnPClient(IMqttConnection c)
        {
            Connection = c;
            getShadowBinder = new GetShadowBinder(c);
            updateShadowBinder = new UpdateShadowBinder(c);
        }

        public Task<string> GetStateAsync(CancellationToken cancellationToken = default) =>
          getShadowBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<string> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) =>
            updateShadowBinder.ReportPropertyAsync(payload, cancellationToken);
    }
}
