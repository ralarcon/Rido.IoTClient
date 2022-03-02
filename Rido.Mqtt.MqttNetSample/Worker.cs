using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rido.Mqtt.HubClient;
using Rido.Mqtt.M2MAdapter;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.Mqtt.MqttNetSample
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
            //var adapter = await MqttNetClient.CreateAsync(_configuration.GetConnectionString("cs"),stoppingToken);
            var adapter = await M2MClientConnectionFactory.CreateHubClientAsync(_configuration.GetConnectionString("cs"), stoppingToken);

            var client = new HubMqttClient(adapter);
            Console.WriteLine("Connected: " + client.ConnectionSettings.ToString());

            client.Command.OnCmdDelegate = async m =>
            {
                _logger.LogInformation("Processing command: " + m.CommandName);
                return await Task.FromResult(new GenericCommandResponse()
                {
                    Status = 200,
                    ReponsePayload = JsonSerializer.Serialize(new { myResponse = "whatever" })
                });
            };

            client.genericDesiredUpdateProperty.OnProperty_Updated = async m =>
            {
                _logger.LogInformation("Processing desired: " + m.ToJsonString());
                return await Task.FromResult(new GenericPropertyAck
                {
                    Value = m.ToJsonString(),
                    Status = 200,
                    Version = m["$version"].GetValue<int>()
                });
            };

            var v = await client.ReportPropertyAsync(new { started = DateTime.Now }, stoppingToken);
            var twin = await client.GetTwinAsync();

            Console.WriteLine();
            Console.WriteLine(twin);
            Console.WriteLine();
            while (!stoppingToken.IsCancellationRequested)
            {
                int puback = await client.SendTelemetryAsync(new { temperature = 23 });
                _logger.LogInformation($"Telemetry pubAck {puback}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
