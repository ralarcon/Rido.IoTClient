﻿//  <auto-generated/> 

using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.Aws;
using Rido.IoTClient.Aws.TopicBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dtmi_rido_pnp
{
    public class memmon_1 : PnPClient
    {
        public Rido.IoTClient.Hive.TopicBindings.Telemetry<double> Telemetry_workingSet;
        public Rido.IoTClient.Hive.TopicBindings.Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response> Command_getRuntimeStats;

        public ReadOnlyProperty<DateTime> Property_started;
        public WritableProperty<bool> Property_enabled;
        public WritableProperty<int> Property_interval;

        private memmon_1(IMqttClient c, ConnectionSettings cs) : base(c, cs)
        {
            Property_started = new ReadOnlyProperty<DateTime>(c, "started");
            Property_interval = new WritableProperty<int>(c, "interval");
            Property_enabled = new WritableProperty<bool>(c, "enabled");
            Telemetry_workingSet = new Rido.IoTClient.Hive.TopicBindings.Telemetry<double>(c, "workingSet");
            Command_getRuntimeStats = new Rido.IoTClient.Hive.TopicBindings.Command<Cmd_getRuntimeStats_Request, Cmd_getRuntimeStats_Response>(c, "getRuntimeStats");
        }

        public static async Task<memmon_1> CreateClientAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionString)
            {
                //ModelId = modelId
            };
            var mqtt = await PnPClient.CreateAsync(cs, cancellationToken);
            var client = new memmon_1(mqtt.Connection, cs);
            client.InitialTwin = await client.GetShadowAsync(cancellationToken);
            return client;
        }
    }

    public enum DiagnosticsMode
    {
        minimal = 0,
        complete = 1,
        full = 2
    }

    public class Cmd_getRuntimeStats_Request : IBaseCommandRequest<Cmd_getRuntimeStats_Request>
    {
        public DiagnosticsMode DiagnosticsMode { get; set; }

        public Cmd_getRuntimeStats_Request DeserializeBody(string payload)
        {
            return new Cmd_getRuntimeStats_Request()
            {
                DiagnosticsMode = System.Text.Json.JsonSerializer.Deserialize<DiagnosticsMode>(payload)
            };
        }
    }

    public class Cmd_getRuntimeStats_Response : BaseCommandResponse
    {
        public Dictionary<string, string> diagnosticResults { get; set; } = new Dictionary<string, string>();
    }
}
