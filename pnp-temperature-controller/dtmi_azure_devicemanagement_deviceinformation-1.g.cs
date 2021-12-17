﻿//  <auto-generated/> 
using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub.TopicBindings;

namespace dtmi_azure_devicemanagement
{

    public class DeviceInformation : Component
    {
        public DeviceInformation(IMqttClient c, string name) : base(c, name)
        {
            Property_manufacturer = new ReadOnlyProperty<string>(c, "manufacturer");
            Property_model = new ReadOnlyProperty<string>(c, "model");
            swVersion = new ReadOnlyProperty<string>(c, "swVersion");
            Property_osName = new ReadOnlyProperty<string>(c, "osName");
            Property_processorArchitecture = new ReadOnlyProperty<string>(c, "processorArchitecture");
            Property_processorManufacturer = new ReadOnlyProperty<string>(c, "processorManufacturer");
            Property_totalStorage = new ReadOnlyProperty<long>(c, "totalStorage");
            Property_totalMemory = new ReadOnlyProperty<long>(c, "totalMemory");
        }

        public ReadOnlyProperty<string> Property_manufacturer { get; set; } 
        public ReadOnlyProperty<string> Property_model { get; set; }
        public ReadOnlyProperty<string> swVersion { get; set; }
        public ReadOnlyProperty<string> Property_osName { get; set; }
        public ReadOnlyProperty<string> Property_processorArchitecture { get; set; }
        public ReadOnlyProperty<string> Property_processorManufacturer { get; set; }
        public ReadOnlyProperty<long> Property_totalMemory { get; set; }
        public ReadOnlyProperty<long> Property_totalStorage { get; set; }


        public override Dictionary<string, object> ToJsonDict()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(Property_manufacturer.Name, Property_manufacturer.PropertyValue);
            dic.Add(Property_model.Name, Property_model.PropertyValue);
            dic.Add(swVersion.Name, swVersion.PropertyValue);
            dic.Add(Property_osName.Name, Property_osName.PropertyValue);
            dic.Add(Property_processorArchitecture.Name, Property_processorArchitecture.PropertyValue);
            dic.Add(Property_processorManufacturer.Name, Property_processorManufacturer.PropertyValue);
            dic.Add(Property_totalMemory.Name, Property_totalMemory.PropertyValue);
            dic.Add(Property_totalStorage.Name, Property_totalStorage.PropertyValue);
            return dic;
        }
    }
}
