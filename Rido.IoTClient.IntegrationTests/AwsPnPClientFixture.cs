﻿using MQTTnet;
using MQTTnet.Client;
using Rido.IoTClient.Aws;
using System.Threading.Tasks;
using Xunit;

namespace Rido.IoTClient.IntegrationTests
{
    public class AwsPnPClientFixture
    {
        [Fact]
        public async Task Connect()
        {
            ConnectionSettings cs = new()
            {
                HostName = "a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com",
                Auth = "X509",
                ClientId = "ridodevice01",
                X509Key = "ridodevice01.pfx|1234"
            };
            PnPClient client = await PnPClient.CreateAsync(cs);
            Assert.True(client.Connection.IsConnected);
            await client.Connection.DisconnectAsync(new MQTTnet.Client.MqttClientDisconnectOptions() { Reason = MQTTnet.Client.MqttClientDisconnectReason.NormalDisconnection });
            Assert.False(client.Connection.IsConnected);
        }

        [Fact]
        public async Task PubSub()
        {
            ConnectionSettings cs = new()
            {
                HostName = "a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com",
                Auth = "X509",
                ClientId = "ridodevice01",
                DeviceId = "ridodevice01",
                X509Key = "ridodevice01.pfx|1234"
            };
            IMqttClient connection = new MqttFactory().CreateMqttClient();
            await connection.ConnectAsync(new MqttClientOptionsBuilder().WithAwsX509Credentials(cs).Build());

            await connection.SubscribeAsync("topic_1");
            bool received = false;
            connection.ApplicationMessageReceivedAsync += async m =>
            {
                received = true;
                await Task.Yield();
            };
            await connection.PublishAsync("topic_1", "{ \"hello\" : 123}");
            await Task.Delay(100);
            Assert.True(received);
        }

        [Fact]
        public async Task GetShadow()
        {
            ConnectionSettings cs = new()
            {
                HostName = "a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com",
                Auth = "X509",
                ClientId = "ridodevice02",
                DeviceId = "ridodevice02",
                X509Key = "ridodevice01.pfx|1234"
            };
            PnPClient client = await PnPClient.CreateAsync(cs);
            Assert.True(client.Connection.IsConnected);
            var shadow = await client.GetShadowAsync();
            Assert.NotNull(shadow);
        }

        [Fact]
        public async Task UpdateShadow()
        {
            ConnectionSettings cs = new()
            {
                HostName = "a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com",
                Auth = "X509",
                ClientId = "ridodevice02",
                DeviceId = "ridodevice02",
                X509Key = "ridodevice01.pfx|1234"
            };
            PnPClient client = await PnPClient.CreateAsync(cs);
            Assert.True(client.Connection.IsConnected);
            var shadow = await client.GetShadowAsync();
            Assert.NotNull(shadow);
            var updRes = await client.UpdateShadowAsync(new
            {
                name = "rido2"
            });
            Assert.True(updRes > 0);
        }

        //[Fact]
        //public async Task ReceiveShadowUpdate()
        //{
        //    ConnectionSettings cs = new()
        //    {
        //        HostName = "a38jrw6jte2l2x-ats.iot.us-west-2.amazonaws.com",
        //        ClientId = "test-shadow",
        //        DeviceId = "TheThing",
        //        Auth = "X509",
        //        X509Key = "TheThing.pfx|1234"
        //    };
        //    PnPClient client = await PnPClient.CreateAsync(cs);
        //    bool received = false;
        //    client.desiredUpdatePropertyBinder.OnProperty_Updated = async m =>
        //    {
        //        received = true;
        //        return await Task.FromResult(new PropertyAck<string>("name")
        //        {
        //            Status = 200,
        //            Version = m.Version,
        //            Value = m.Value

        //        });
        //    };
        //    Assert.True(client.Connection.IsConnected);
        //    var updRes = await client.UpdateShadowAsync(new
        //    {
        //        name = "rido2"
        //    });
        //    Assert.True(received);
        //}


    }
}
