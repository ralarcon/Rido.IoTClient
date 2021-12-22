using Rido.Mqtt.IoTHubPnPClient;
using Rido.Mqtt.PnPApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rido.Mqtt.Tests.SampleDevices
{
    public class OneReadOnlyPropertyClient : BaseIoTHubClient
    {
        public const string ModelId = "dtmi:com:a;1";
        public ReadOnlyProperty<int> Property_myInt;
        public OneReadOnlyPropertyClient(IMqttConnection connection, ConnectionSettings cs) : base(connection, cs)
        {
            Property_myInt = new ReadOnlyProperty<int>(base.updateTwinBinder, "myInt");
        }
    }
}
