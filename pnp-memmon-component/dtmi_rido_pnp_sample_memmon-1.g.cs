﻿//  <auto-generated/> 

using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;

namespace dtmi_rido_pnp_sample
{
    public class memmon : HubClient
    {
        const string modelId = "dtmi:rido:pnp:sample:memmon;1";

        public ReadOnlyProperty<DateTime> Property_memMon_started { get; private set; }
        public WritableProperty<bool> Property_memMon_enabled;
        public WritableProperty<int> Property_memMon_interval;
        public Telemetry<double> Telemetry_memMon_workingSet;
        public Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats_Binder;
        public Component<dtmi_azure_devicemanagement.DeviceInformation> Component_deviceInfo;

        private memmon(IMqttClient c) : base(c)
        {
            Property_memMon_started = new ReadOnlyProperty<DateTime>(c, "started", "memMon");
            Property_memMon_enabled = new WritableProperty<bool>(c, "enabled", "memMon");
            Property_memMon_interval = new WritableProperty<int>(c, "interval", "memMon");
            Telemetry_memMon_workingSet = new Telemetry<double>(c, "workingSet", "memMon");
            Command_getRuntimeStats_Binder = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats", "memMon");
            Component_deviceInfo = new Component<dtmi_azure_devicemanagement.DeviceInformation>(c, "deviceInfo");
        }

        public static async Task<memmon> CreateDeviceClientAsync(string connectionString, CancellationToken cancellationToken)
        {
            var cs = new ConnectionSettings(connectionString) { ModelId = modelId };
            var mqtt = await HubClient.CreateAsync(cs, cancellationToken);
            var client = new memmon(mqtt.Connection) { ConnectionSettings = cs };
            client.InitialTwin = await client.GetTwinAsync();
            return client;
        }
    }
}
