//using Rido.Mqtt.IoTHubPnPClient;
using Rido.Mqtt.MqttNetPnPAdapter;
using Rido.Mqtt.PnPApi;
using Rido.Mqtt.Tests.SampleDevices;

namespace MqttNetRunner
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;


        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = await CreateClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);

            var res = await client.Property_myInt.ReportPropertyAsync(33,false, stoppingToken);
            Console.WriteLine(res);
            
            var twin = await client.GetTwinAsync(stoppingToken);
            Console.WriteLine(twin);
            
            _logger.LogInformation("The End");
        }

        static async Task<OneReadOnlyPropertyClient> CreateClientAsync(string connectionString, CancellationToken token)
        {
            var cs = new ConnectionSettings(connectionString)
            {
                ModelId = OneReadOnlyPropertyClient.ModelId
            };
            (string u, string p) = SasAuth.GenerateHubSasCredentials(cs.HostName, cs.DeviceId, cs.SharedAccessKey, cs.ModelId, cs.SasMinutes);
            var connection = await MqttNetConnection.CreateAsync(cs.HostName, cs.DeviceId, u, p, token);
            return new OneReadOnlyPropertyClient(connection) { ConnectionSettings = cs }; 
        }
    }
}