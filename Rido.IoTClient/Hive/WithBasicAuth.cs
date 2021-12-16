﻿using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rido.IoTClient.Hive
{
    public static class MqttNetExtensions
    {
        public static MqttClientOptionsBuilder WithBasicAuth(this MqttClientOptionsBuilder builder, ConnectionSettings cs)
        {
            builder
             .WithTcpServer(cs.HostName, 8883)
                .WithTls()
                .WithClientId(cs.DeviceId)
                .WithCredentials(cs.DeviceId, cs.SharedAccessKey);
            return builder;
        }
    }
}
