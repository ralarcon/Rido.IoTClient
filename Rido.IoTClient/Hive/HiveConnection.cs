﻿using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Hive
{
    public class HiveClient
    {
        public IMqttClient Connection;

        public ConnectionSettings ConnectionSettings;

        protected static async Task<IMqttClient> CreateAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            IMqttClient mqtt = new MqttFactory(MqttNetTraceLogger.CreateTraceLogger()).CreateMqttClient(new MqttClientAdapterFactory());
            var connAck = await mqtt.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer(cs.HostName, 8884)
                .WithTls()
                .WithClientId(cs.DeviceId)
                .WithCredentials(cs.DeviceId, cs.SharedAccessKey)
                .Build(), cancellationToken);
            if (connAck.ResultCode != MqttClientConnectResultCode.Success)
            {
                Trace.TraceError(connAck.ReasonString);
                throw new ApplicationException("Error connecting to MQTT endpoint. " + connAck.ReasonString);
            }
            return mqtt;
        }
    }
}
