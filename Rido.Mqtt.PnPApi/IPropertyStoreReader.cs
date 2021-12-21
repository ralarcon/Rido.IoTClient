namespace Rido.Mqtt.PnPApi
{
    public interface IPropertyStoreReader
    {
        Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default);
    }
}