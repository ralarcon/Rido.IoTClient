namespace Rido.Mqtt.PnPApi
{
    public interface IPropertyUpdateCallback
    {
        Func<string, Task<string>> OnProperty_Updated { get; set; }
    }

    public interface IPropertyUpdateCallback<T>
    {
        Func<PropertyAck<T>, Task<PropertyAck<T>>> OnProperty_Updated { get; set; }
    }
}
