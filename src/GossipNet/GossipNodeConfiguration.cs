using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Messages;
using Serilog;

namespace GossipNet
{
    public class GossipNodeConfiguration
    {
        private GossipNodeConfiguration(Builder builder)
        {
            BroadcastFrequency = builder.BroadcastFrequency ?? TimeSpan.FromSeconds(2);
            CompressionType = builder.CompressionType;
            LocalEndPoint = builder.LocalEndPoint ?? new IPEndPoint(IPAddress.Loopback, 29803);
            Logger = (builder.LoggerConfiguration ?? new LoggerConfiguration())
                .Destructure.AsScalar<IPEndPoint>()
                .CreateLogger();
            Metadata = builder.Metadata;
            Name = builder.Name ?? LocalEndPoint.ToString();
        }

        public TimeSpan BroadcastFrequency { get; private set; }

        public CompressionType? CompressionType { get; private set; }

        public IPEndPoint LocalEndPoint { get; private set; }

        public ILogger Logger { get; private set; }

        public byte[] Metadata { get; private set; }

        public string Name { get; private set; }

        public static GossipNodeConfiguration Create(Action<Builder> configure)
        {
            var builder = new Builder();
            configure(builder);
            return new GossipNodeConfiguration(builder);
        }

        public class Builder
        {
            public TimeSpan? BroadcastFrequency { get; set; }

            public CompressionType? CompressionType { get; set; }

            public IPEndPoint LocalEndPoint { get; set; }

            public LoggerConfiguration LoggerConfiguration { get; set; }

            public byte[] Metadata { get; set; }

            public string Name { get; set; }
        }
    }
}