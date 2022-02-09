﻿using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.AzBroker.TopicBindings;
using Rido.IoTClient.AzDps;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzBroker
{
    public class IoTHubBrokerClient : BaseClient
    {
      
        readonly IPropertyStoreReader getTwinBinder;
        readonly IPropertyStoreWriter updateTwinBinder;

        public IoTHubBrokerClient(IMqttClient c) : base(c)
        {
            getTwinBinder = new GetTwinBinder(c);
            updateTwinBinder = new UpdateTwinBinder(c);
        }

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) =>
            getTwinBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) =>
            updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

    }
}