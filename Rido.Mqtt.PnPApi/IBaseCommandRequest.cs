namespace Rido.Mqtt.PnPApi
{
    public interface IBaseCommandRequest<T>
    {
        public T DeserializeBody(string payload);
    }
}
