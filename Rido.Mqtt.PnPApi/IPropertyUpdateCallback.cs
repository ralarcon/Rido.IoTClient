namespace Rido.Mqtt.PnPApi
{
    public interface IPropertyUpdateCallback<T>
    {
        Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated { get; set; }
    }
}
