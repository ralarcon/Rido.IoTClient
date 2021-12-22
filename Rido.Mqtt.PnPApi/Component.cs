namespace Rido.Mqtt.PnPApi
{
    public abstract class Component
    {
        private readonly string name;
        private readonly IPropertyStoreWriter update;

        public Component(IPropertyStoreWriter updater, string name)
        {
            this.name = name;
            update = updater;
        }

        public async Task<string> ReportPropertyAsync(CancellationToken token = default)
        {
            Dictionary<string, Dictionary<string, object>> dict = new()
            {
                { name, new Dictionary<string, object>() }
            };
            dict[name] = ToJsonDict();
            dict[name].Add("__t", "c");
            var v = await update.ReportPropertyAsync(dict, token);
            return v;
        }

        public abstract Dictionary<string, object> ToJsonDict();
    }
}
