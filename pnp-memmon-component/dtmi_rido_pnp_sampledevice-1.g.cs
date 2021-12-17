﻿//  <auto-generated/> 

using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;
//using Rido.IoTClient.Hive;
//using Rido.IoTClient.Hive.TopicBindings;

namespace dtmi_rido_pnp
{
    public class sampleDevice : IoTHubPnPClient
    {
        const string modelId = "dtmi:rido:pnp:sampleDevice;1";

        public DeviceInformationComponent Component_deviceInfo;
        public memMonComponent Component_memMon;
        public ReadOnlyProperty<string> Property_serialNumber;
        public Command<EmptyCommandRequest, EmptyCommandResponse> Command_reboot;

        private sampleDevice(IMqttClient c)  : base(c)
        {
            Component_deviceInfo = new DeviceInformationComponent(c, "deviceInfo");
            Component_memMon = new memMonComponent(c, "memMon");
            Property_serialNumber = new ReadOnlyProperty<string>(c, "serialNumber");
            Command_reboot = new Command<EmptyCommandRequest, EmptyCommandResponse>(c, "reboot");
        }

        public static async Task<sampleDevice> CreateDeviceClientAsync(string connectionString, CancellationToken cancellationToken)
        {
            var cs = new ConnectionSettings(connectionString) { ModelId = modelId };
            var connection = await IoTHubConnectionFactory.CreateAsync(cs, cancellationToken);
            var client = new sampleDevice(connection) { ConnectionSettings = cs };
            client.InitialTwin = await client.GetTwinAsync();
            return client;
        }
    }
}
