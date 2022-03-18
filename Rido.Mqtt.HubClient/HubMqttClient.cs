﻿using Rido.Mqtt.DpsClient;
using Rido.Mqtt.HubClient.TopicBindings;
using Rido.MqttCore;
using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.HubClient
{
    public class HubMqttClient : IHubMqttClient
    {
        public IMqttBaseClient Connection { get; set; }

        private readonly IPropertyStoreReader getTwinBinder;
        private readonly IReportPropertyBinder updateTwinBinder;
        private readonly GenericDesiredUpdatePropertyBinder genericDesiredUpdateProperty;
        private readonly GenericCommand command;

        public static async Task<HubMqttClient> CreateFromConnectionStringAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cs = new ConnectionSettings(connectionString);
            if (string.IsNullOrEmpty(cs.HostName) && !string.IsNullOrEmpty(cs.IdScope))
            {
                var dpsMqtt = await new Rido.Mqtt.MqttNet3Adapter.MqttNetClientConnectionFactory().CreateDpsClientAsync(connectionString, cancellationToken);
                var dpsClient = new MqttDpsClient(dpsMqtt);
                var dpsRes = await dpsClient.ProvisionDeviceIdentity();
                cs.HostName = dpsRes.RegistrationState.AssignedHub;
                await dpsMqtt.DisconnectAsync(cancellationToken);
            }
            var mqtt = await new MqttNet3Adapter.MqttNetClientConnectionFactory().CreateHubClientAsync(cs);
            return new HubMqttClient(mqtt);
        }

        public HubMqttClient(IMqttBaseClient c)
        {
            Connection = c;
            getTwinBinder = new GetTwinBinder(c);
            updateTwinBinder = new UpdateTwinBinder(c);
            command = new GenericCommand(c);
            genericDesiredUpdateProperty = new GenericDesiredUpdatePropertyBinder(c);
        }

        public Func<GenericCommandRequest, Task<CommandResponse>> OnCommandReceived
        {
            get => command.OnCmdDelegate;
            set => command.OnCmdDelegate = value;
        }

        public Func<JsonNode, Task<GenericPropertyAck>> OnPropertyUpdateReceived
        {
            get => genericDesiredUpdateProperty.OnProperty_Updated;
            set => genericDesiredUpdateProperty.OnProperty_Updated = value;
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) => getTwinBinder.ReadPropertiesDocAsync(cancellationToken);
        public Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) => updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);
        public Task<int> SendTelemetryAsync(object payload, CancellationToken t = default) => Connection.PublishAsync($"devices/{Connection.ClientId}/messages/events/", payload, 1, t);
    }
}
