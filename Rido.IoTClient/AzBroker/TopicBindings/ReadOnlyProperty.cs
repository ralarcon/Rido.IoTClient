﻿using MQTTnet.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzBroker.TopicBindings
{
    public class ReadOnlyProperty<T> : IReadOnlyProperty<T>
    {
        readonly IPropertyStoreWriter updateTwin;
        readonly string name;
        readonly string component;

        public T PropertyValue { get; set; }
        public int Version { get; set; }

        public ReadOnlyProperty(IMqttClient connection, string name, string component = "")
        {
            updateTwin = new UpdateTwinBinder(connection);
            this.name = name;
            this.component = component;
        }

        public async Task<int> ReportPropertyAsync(CancellationToken cancellationToken = default)
        {
            bool asComponent = !string.IsNullOrEmpty(component);
            return await updateTwin.ReportPropertyAsync(ToJsonDict(asComponent), cancellationToken);
        }

        Dictionary<string, object> ToJsonDict(bool asComponent = false)
        {
            Dictionary<string, object> result;
            if (asComponent == false)
            {
                result = new Dictionary<string, object> { { name, PropertyValue } };
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>>
                {
                    { component, new Dictionary<string, object>() }
                };
                dict[component].Add("__t", "c");
                dict[component].Add(name, PropertyValue);
                result = dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
            }
            return result;
        }

    }
}
