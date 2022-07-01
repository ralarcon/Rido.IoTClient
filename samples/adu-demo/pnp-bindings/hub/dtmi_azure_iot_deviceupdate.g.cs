﻿//  <auto-generated/> 

using Rido.Mqtt.HubClient.TopicBindings;
using Rido.MqttCore;
using Rido.MqttCore.PnP;
using static dtmi.azure.iot.deviceupdate;

namespace adu_demo_pnp_bindings_hub
{
    public class deviceupdate : Component, dtmi.azure.iot.deviceupdate
    {
        public IWritableProperty<agentMetadata> Property_agent { get; set; }
        public IWritableProperty<serviceMetadata> Property_service { get; set ; }
        public deviceupdate(IMqttBaseClient c, string name) : base(c, name)
        {
            Property_agent = new WritableProperty<agentMetadata>(c, "agent", name);
            Property_service = new WritableProperty<serviceMetadata>(c, "service", name);
            Property_agent.PropertyValue.Value = new agentMetadata();
            Property_service.PropertyValue.Value = new serviceMetadata();
        }
        public override Dictionary<string, object> ToJsonDict()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            return dict;
        }
    }
}