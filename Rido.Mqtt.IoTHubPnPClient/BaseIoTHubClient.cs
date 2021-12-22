using Rido.Mqtt.PnPApi;

namespace Rido.Mqtt.IoTHubPnPClient
{
    public class BaseIoTHubClient
    {
        public readonly IMqttConnection Connection;

        public ConnectionSettings ConnectionSettings;
        public string InitialState = string.Empty;

        protected readonly IPropertyStoreReader getTwinBinder;
        protected readonly IPropertyStoreWriter updateTwinBinder;

        public BaseIoTHubClient(IMqttConnection connection)
        {
            Connection = connection;
            getTwinBinder = new GetTwinBinder(connection);
            updateTwinBinder = new UpdateTwinBinder(connection);
        }   

        public Task<string> GetTwinAsync(CancellationToken cancellationToken = default) =>
            getTwinBinder.ReadPropertiesDocAsync(cancellationToken);

        public Task<string> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default) =>
            updateTwinBinder.ReportPropertyAsync(payload, cancellationToken);

    }
}
