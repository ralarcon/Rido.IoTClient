﻿//  <auto-generated/> 
using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub.TopicBindings;

namespace dtmi_azure_devicemanagement
{
    public class DeviceInformationComponent : Component<DeviceInformation>
    {
        public DeviceInformationComponent(IMqttClient c, string name) : base(c, name)
        {
            ComponentValue.Property_manufacturer = new ReadOnlyProperty<string>(c, "manufacturer");
            ComponentValue.Property_model = new ReadOnlyProperty<string>(c, "model");
            ComponentValue.Property_softwareVersion = new ReadOnlyProperty<string>(c, "softwareVersion");
            ComponentValue.Property_operatingSystemName = new ReadOnlyProperty<string>(c, "operatingSystemName");
            ComponentValue.Property_processorArchitecture = new ReadOnlyProperty<string>(c, "processorArchitecture");
            ComponentValue.Property_processorManufacturer = new ReadOnlyProperty<string>(c, "processorManufacturer");
            ComponentValue.Property_totalMemory = new ReadOnlyProperty<long>(c, "totalMemory");
            ComponentValue.Property_totalStorage = new ReadOnlyProperty<long>(c, "totalStorage");
        }
    }

    public class DeviceInformation : ITwinSerializable
    {
        public ReadOnlyProperty<string> Property_manufacturer { get; set; } 
        public ReadOnlyProperty<string> Property_model { get; set; }
        public ReadOnlyProperty<string> Property_softwareVersion { get; set; }
        public ReadOnlyProperty<string> Property_operatingSystemName { get; set; }
        public ReadOnlyProperty<string> Property_processorArchitecture { get; set; }
        public ReadOnlyProperty<string> Property_processorManufacturer { get; set; }
        public ReadOnlyProperty<long> Property_totalMemory { get; set; }
        public ReadOnlyProperty<long> Property_totalStorage { get; set; }

        public Dictionary<string, object> ToJsonDict()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(Property_manufacturer.Name, Property_manufacturer.PropertyValue);
            dic.Add(Property_model.Name, Property_model.PropertyValue);
            dic.Add(Property_softwareVersion.Name, Property_softwareVersion.PropertyValue);
            dic.Add(Property_operatingSystemName.Name, Property_operatingSystemName.PropertyValue);
            dic.Add(Property_processorArchitecture.Name, Property_processorArchitecture.PropertyValue);
            dic.Add(Property_processorManufacturer.Name, Property_processorManufacturer.PropertyValue);
            dic.Add(Property_totalMemory.Name, Property_totalMemory.PropertyValue);
            dic.Add(Property_totalStorage.Name, Property_totalStorage.PropertyValue);
            return dic;
        }
    }
}
