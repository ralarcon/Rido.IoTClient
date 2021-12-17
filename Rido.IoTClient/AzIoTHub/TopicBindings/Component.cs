﻿using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public abstract class Component 
    {
        readonly string name;
        readonly UpdateTwinBinder update;

        public Component(IMqttClient connection, string name)
        {
            this.name = name;
            update = UpdateTwinBinder.GetInstance(connection);
        }

        public async Task<int> ReportPropertyAsync(CancellationToken token)
        {
            Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { name, new Dictionary<string, object>() }
                };
            dict[name] = this.ToJsonDict();
            dict[name].Add("__t", "c");
            var v = await update.ReportPropertyAsync(dict, token);
            return v;
        }

        public abstract Dictionary<string, object> ToJsonDict();
    }
}
