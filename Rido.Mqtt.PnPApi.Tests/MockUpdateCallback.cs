using Rido.Mqtt.PnPApi;
using System;
using System.Threading.Tasks;

namespace Rido.Mqtt.PnPApi.Tests
{
    internal class MockUpdateCallback<T> : IPropertyUpdateCallback<T>
    {


        public Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated { get; set; }
    }
}