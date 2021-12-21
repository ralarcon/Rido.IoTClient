using Rido.Mqtt.PnPApi;
using Rido.Mqtt.PnPApi.Binders;
using System.Text.Json;

namespace Rido.Mqtt.IoTHubPnPClient
{
    public class Command<T, TResponse>
        where T : IBaseCommandRequest<T>, new()
        where TResponse : BaseCommandResponse
    {
        public Func<T, Task<TResponse>> OnCmdDelegate { get; set; }

        public Command(IMqttConnection connection, string commandName, string componentName = "")
        {
            var fullCommandName = string.IsNullOrEmpty(componentName) ? commandName : $"{componentName}*{commandName}";

            var binder = new RequestResponseBinder(
                connection,
                $"$iothub/methods/POST/",
                "$iothub/methods/res/200/?$rid={rid}",
                fullCommandName)
            {
                RequestHandler = ProcessCommand
            };
        }

        private async Task<string> ProcessCommand(string requestJson)
        {
            string result = "{}";
            T req = new T().DeserializeBody(requestJson);
            if (OnCmdDelegate != null && req != null)
            {
                TResponse response = await OnCmdDelegate.Invoke(req);
                if (response != null)
                {
                    result = JsonSerializer.Serialize(response);
                }
            }
            return result;
        }
    }
}
