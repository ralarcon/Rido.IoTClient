﻿//  <auto-generated/> 

using MQTTnet.Client;
using pnp_memmon;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;

namespace dtmi_rido_pnp_IoTHubClassic
{
    public class memmon : IoTHubClient, Imemmon
    {
        const string modelId = "dtmi:rido:pnp:memmon;1";

        public IReadOnlyProperty<DateTime> Property_started { get; set; }
        public IWritableProperty<bool> Property_enabled { get; set; }
        public IWritableProperty<int> Property_interval { get; set; }
        public ITelemetry<double> Telemetry_workingSet { get; set; }
        public ICommand<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats { get; set; }

        private memmon(IMqttClient c) : base(c)
        {
            Property_started = new ReadOnlyProperty<DateTime>(c, "started");
            Property_interval = new WritableProperty<int>(c, "interval");
            Property_enabled = new WritableProperty<bool>(c, "enabled");
            Telemetry_workingSet = new Telemetry<double>(c, "workingSet");
            Command_getRuntimeStats = new Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats");
        }

        public static async Task<memmon> CreateClientAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionString) { ModelId = modelId };
            var client = new memmon(await IoTHubConnectionFactory.CreateAsync(cs, cancellationToken)) { ConnectionSettings = cs };
            client.InitialState = await client.GetTwinAsync(cancellationToken);
            return client;
        }
    }

   
}
