﻿using MQTTnet.Client;
using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Rido.Mqtt.MqttNet4Adapter
{
    public static class WithAwsX509CredentialsExtension
    {
        public static MqttClientOptionsBuilder WithAwsX509Credentials(this MqttClientOptionsBuilder builder, ConnectionSettings cs)
        {
            var segments = cs.X509Key.Split('|');
            var cert = new X509Certificate2(segments[0], segments[1], X509KeyStorageFlags.Exportable);
            Trace.TraceInformation($"Loaded cert {cert.Subject} {cert.Thumbprint}");
            builder
                .WithTcpServer(cs.HostName, 8883)
                .WithKeepAlivePeriod(new TimeSpan(0, 0, 0, 300))
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                .WithClientId(cs.ClientId)
                .WithCleanSession(false)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    AllowUntrustedCertificates = false,
                    IgnoreCertificateChainErrors = false,
                    IgnoreCertificateRevocationErrors = false,
                    SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                    Certificates = new List<X509Certificate> { cert },
                });
            return builder;
        }
    }
}
