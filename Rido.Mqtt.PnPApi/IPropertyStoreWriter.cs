namespace Rido.Mqtt.PnPApi
{
    public interface IPropertyStoreWriter
    {
        Task<string> ReportPropertyAsync(object payload, CancellationToken token = default);
    }
}
