namespace Rido.Mqtt.PnPApi
{
    public interface IPropertyStoreWriter<T>
    {
        Task<string> ReportPropertyAsync(object payload, CancellationToken token = default);
    }
}
