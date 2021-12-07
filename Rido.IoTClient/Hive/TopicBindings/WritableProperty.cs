﻿using MQTTnet.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Hive.TopicBindings
{
    public class WritableProperty<T>
    {
        public PropertyAck<T> PropertyValue;
        readonly string propertyName;
        readonly string componentName;
        //readonly UpdateTwinBinder updateTwin;
        readonly UpdatePropertyBinder updatePropertyBinder;
        readonly DesiredUpdatePropertyBinder<T> desiredBinder;

        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated
        {
            get => desiredBinder.OnProperty_Updated;
            set => desiredBinder.OnProperty_Updated = value;
        }

        public WritableProperty(IMqttClient connection, string name, string component = "")
        {
            propertyName = name;
            componentName = component;
            //updateTwin = new UpdateTwinBinder(connection);
            updatePropertyBinder = new UpdatePropertyBinder(connection);
            PropertyValue = new PropertyAck<T>(name, componentName);
            desiredBinder = new DesiredUpdatePropertyBinder<T>(connection, name, componentName);
        }

        public async Task UpdatePropertyAsync() => await updatePropertyBinder.ReportProperty(this.PropertyValue.ToAck());

        public async Task InitPropertyAsync(string twin, T defaultValue, CancellationToken cancellationToken = default)
        {
            PropertyValue = new PropertyAck<T>(propertyName, componentName)
            {
                Value = defaultValue
            };
            _ = await updatePropertyBinder.ReportProperty(PropertyValue.ToAck());
        }
    }
}
