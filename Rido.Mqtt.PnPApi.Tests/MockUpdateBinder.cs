
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.PnPApi.Tests
{
    internal class MockUpdateBinder : IPropertyStoreWriter
    {
        internal object ReceivedPayload { get; set; }

        public async Task<string> ReportPropertyAsync(object payload, CancellationToken token = default)
        {
            ReceivedPayload = (Dictionary<string, object>)payload;
            return await Task.FromResult("");
        }
    }
}