﻿using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rido.IoTClient.AzIoTHub.TopicBindings
{
    public class GetTwinBinder : IPropertyStoreReader
    {
        readonly static ConcurrentDictionary<int, TaskCompletionSource<string>> pendingGetTwinRequests = new ConcurrentDictionary<int, TaskCompletionSource<string>>();
        readonly IMqttClient connection;

        private static GetTwinBinder instance;
        public static GetTwinBinder GetInstance(IMqttClient connection)
        {
            if (instance == null || instance.connection != connection)
            {
                instance = new GetTwinBinder(connection);
            }
            return instance;
        }

        private GetTwinBinder(IMqttClient conn)
        {
            connection = conn;
            _ = connection.SubscribeAsync("$iothub/twin/res/200");
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                var topic = m.ApplicationMessage.Topic;

                if (topic.StartsWith("$iothub/twin/res/200"))
                {
                    string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                    (int rid, _) = TopicParser.ParseTopic(topic);
                    if (pendingGetTwinRequests.TryRemove(rid, out var tcs))
                    {
                        tcs.SetResult(msg);
                    }
                }
                await Task.Yield();
            };
        }

        public async Task<string> ReadPropertiesDocAsync(CancellationToken cancellationToken = default)
        {
            var rid = RidCounter.NextValue();
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var puback = await connection.PublishAsync(new MqttApplicationMessage()
            {
                Topic = $"$iothub/twin/GET/?$rid={rid}",
            }, cancellationToken);

            if (puback?.ReasonCode == MqttClientPublishReasonCode.Success)
            {
                pendingGetTwinRequests.TryAdd(rid, tcs);
            }
            else
            {
                Trace.TraceError($"Error '{puback?.ReasonCode}' publishing twin GET");
            }
            return await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(10));
        }

    }
}
