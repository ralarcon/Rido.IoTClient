namespace Rido.Mqtt.PnPApi
{
    public class MqttMessage
    {
        public string Topic { get; set; } = "";
        public string Payload { get; set; } = "";
    }

    public interface IMqttConnection
    {
        bool IsConnected { get; }
        string ClientId { get; }
        Task<int> PublishAsync(string topic, string payload, int qos = 0, CancellationToken token = default);
        Task<int> SubscribeAsync(string topic, CancellationToken token = default);
        event EventHandler<DisconnectEventArgs> OnMqttClientDisconnected;
        event Func<MqttMessage, Task> OnMessage;
    }

    public class DisconnectEventArgs
    {
        public string ReasonInfo { get; set; } = string.Empty;
    }
}
