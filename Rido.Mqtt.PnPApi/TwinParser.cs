using System.Text.Json.Nodes;

namespace Rido.Mqtt.PnPApi
{
    public class TwinParser
    {
        public static JsonNode ReadPropertyFromDesired(JsonNode desired, string propertyName, string componentName)
        {
            JsonNode result = null;
            if (string.IsNullOrEmpty(componentName))
            {
                result = desired?[propertyName];
            }
            else
            {
                if (desired[componentName] != null &&
                    desired[componentName]?[propertyName] != null &&
                    desired[componentName]?["__t"] != null &&
                    desired[componentName]?["__t"]?.GetValue<string>() == "c")
                {
                    result = desired?[componentName]?[propertyName];
                }
            }

            return result;
        }

        public JsonNode ReadPropertyFromDesired(JsonNode desired, object propertyName, object componentName)
        {
            throw new NotImplementedException();
        }
    }
}
