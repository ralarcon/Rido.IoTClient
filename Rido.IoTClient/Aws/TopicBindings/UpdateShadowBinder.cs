﻿using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.Aws.TopicBindings
{
    public class UpdateShadowBinder : IReportPropoertyBinder
    {
        TaskCompletionSource<int> pendingRequest;
        readonly IMqttClient connection;

        private static UpdateShadowBinder instance;

        public static UpdateShadowBinder GetInstance(IMqttClient c)
        {
            if (instance == null || instance.connection != c)
            {
                instance = new UpdateShadowBinder(c);
            }
            return instance;
        }
        UpdateShadowBinder(IMqttClient connection)
        {
            this.connection = connection;
            _ = connection.SubscribeAsync($"$aws/things/{connection.Options.ClientId}/shadow/update/+");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;
                if (topic.StartsWith($"$aws/things/{connection.Options.ClientId}/shadow/update/accepted"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    JsonNode node = JsonNode.Parse(msg);
                    int version = node["version"].GetValue<int>();
                    if (pendingRequest != null && !pendingRequest.Task.IsCompleted)
                    {
                        pendingRequest.SetResult(version);
                    }
                }
                if (topic.StartsWith($"$aws/things/{connection.Options.ClientId}/shadow/update/rejected"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    if (pendingRequest != null && !pendingRequest.Task.IsCompleted)
                    {
                        pendingRequest.SetException(new ApplicationException(msg));
                    }
                    Trace.TraceWarning(msg);
                }
                await Task.Yield();
            };
        }

        public async Task<int> ReportPropertyAsync(object payload, CancellationToken cancellationToken = default)
        {
            pendingRequest = new TaskCompletionSource<int>();
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "state", new Dictionary<string, object>()
                    {
                       { "reported", payload}
                    }
                }
            };
            var puback = await connection.PublishAsync($"$aws/things/{connection.Options.ClientId}/shadow/update", data, cancellationToken);
            if (puback.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                Trace.TraceError("Error publishing message: " + puback.ReasonString);
                throw new ApplicationException(puback.ReasonString);
            }
            return await pendingRequest.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
        }
    }
}
