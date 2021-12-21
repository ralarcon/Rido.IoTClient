using System.Text.Json.Serialization;

namespace Rido.Mqtt.PnPApi
{
    public abstract class BaseCommandResponse
    {
        [JsonIgnore]
        public int Status { get; set; }
    }
}
