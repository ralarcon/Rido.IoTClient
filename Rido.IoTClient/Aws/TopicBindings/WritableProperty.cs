﻿using MQTTnet.Client;
using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws.TopicBindings
{
    public class WritableProperty<T>
    {
        public PropertyAck<T> PropertyValue;
        readonly string propertyName;
        readonly string componentName;
        //readonly UpdateTwinBinder updateTwin;
        readonly UpdateShadowBinder updatePropertyBinder;
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
            updatePropertyBinder = UpdateShadowBinder.GetInstance(connection);
            PropertyValue = new PropertyAck<T>(name, componentName);
            desiredBinder = new DesiredUpdatePropertyBinder<T>(connection, name, componentName);
        }

        public async Task UpdatePropertyAsync() => await updatePropertyBinder.ReportPropertyAsync(this.PropertyValue.ToAckDict());

        public async Task InitPropertyAsync(string twin, T defaultValue, CancellationToken cancellationToken = default)
        {
            PropertyValue = InitFromTwin(twin, propertyName, componentName, defaultValue);

            if (desiredBinder.OnProperty_Updated != null && (PropertyValue.DesiredVersion > 1))
            {
                var ack = await desiredBinder.OnProperty_Updated.Invoke(PropertyValue);
                _ = updatePropertyBinder.ReportPropertyAsync(ack.ToAckDict(), cancellationToken);
                PropertyValue = ack;
            }
            else
            {
                _ = updatePropertyBinder.ReportPropertyAsync(PropertyValue.ToAckDict());
            }
        }

        PropertyAck<T> InitFromTwin(string twinJson, string propName, string componentName, T defaultValue)
        {
            if (string.IsNullOrEmpty(twinJson))
            {
                return new PropertyAck<T>(propName, componentName) { Value = defaultValue };
            }

            var root = JsonNode.Parse(twinJson);
            var desired = root["state"]["desired"];
            var reported = root["state"]["reported"];
            T desired_Prop = default;
            int desiredVersion = root["version"].GetValue<int>();
            PropertyAck<T> result = new PropertyAck<T>(propName, componentName) { DesiredVersion = desiredVersion };
            bool desiredFound = false;
            if (desired?[propName] != null)
            {
                desired_Prop = desired[propName].GetValue<T>();
                desiredFound = true;
            }

            bool reportedFound = false;
            T reported_Prop = default;
            int reported_Prop_version = 0;
            int reported_Prop_status = 001;
            string reported_Prop_description = String.Empty;
            if (reported?[propName] != null)
            {
                reported_Prop = reported[propName]["value"].GetValue<T>();
                reported_Prop_version = reported[propName]["av"]?.GetValue<int>() ?? -1;
                reported_Prop_status = reported[propName]["ac"].GetValue<int>();
                reported_Prop_description = reported[propName]["ad"]?.GetValue<string>();
                reportedFound = true;
            }

            if (!desiredFound && !reportedFound)
            {
                result = new PropertyAck<T>(propName, componentName)
                {
                    //DesiredVersion = desiredVersion,
                    Version = reported_Prop_version,
                    Value = defaultValue,
                    Status = 203,
                    Description = "Init from default value"
                };
            }

            if (!desiredFound && reportedFound)
            {
                result = new PropertyAck<T>(propName, componentName)
                {
                    DesiredVersion = 0,
                    Version = reported_Prop_version,
                    Value = reported_Prop,
                    Status = reported_Prop_status,
                    Description = reported_Prop_description
                };
            }

            if (desiredFound && reportedFound)
            {
                if (desiredVersion >= reported_Prop_version)
                {
                    result = new PropertyAck<T>(propName, componentName)
                    {
                        DesiredVersion = desiredVersion,
                        Value = desired_Prop,
                        Version = desiredVersion
                    };
                }
            }


            if (desiredFound && !reportedFound)
            {
                result = new PropertyAck<T>(propName, componentName)
                {
                    DesiredVersion = desiredVersion,
                    Version = desiredVersion,
                    Value = desired_Prop
                };
            }

            return result;
        }
    }
}