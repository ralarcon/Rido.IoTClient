using Rido.Mqtt.PnPApi;

namespace Rido.Mqtt.PnPApi
{
    public class ReadOnlyProperty<T>
    {
        private readonly IPropertyStoreWriter<string> updater;
        public readonly string Name;
        private readonly string component;
        public T PropertyValue;
        public string Version;

        public ReadOnlyProperty(IPropertyStoreWriter<string> updater, string name, string component = "")
        {
            this.updater = updater;
            Name = name;
            this.component = component;
        }

        public async Task<string> ReportPropertyAsync(T newValue, bool asComponent = false, CancellationToken cancellationToken = default)
        {
            PropertyValue = newValue;
            Version = await updater.ReportPropertyAsync(ToJsonDict(asComponent), cancellationToken);
            return Version;
        }

        private Dictionary<string, object> ToJsonDict(bool asComponent = false)
        {
            ArgumentNullException.ThrowIfNull(PropertyValue);
            Dictionary<string, object> result;
            if (asComponent == false)
            {
                result = new Dictionary<string, object> { { Name, PropertyValue } };
            }
            else
            {
                Dictionary<string, Dictionary<string, object>> dict = new()
                {
                    { component, new Dictionary<string, object>() }
                };
                dict[component].Add("__t", "c");
                dict[component].Add(Name, PropertyValue);
                result = dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
            }
            return result;
        }

    }
}
