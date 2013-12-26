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
        public IPEndPoint LocalEndPoint { get; private set; }

        public ILogger Logger { get; private set; }

        public byte[] Metadata { get; private set; }

        public string Name { get; private set; }

        private GossipNodeConfiguration(Builder builder)
        {
            LocalEndPoint = builder.LocalEndPoint;
            Logger = builder.Logger;
            Metadata = builder.Metadata;
            Name = builder.Name ?? LocalEndPoint.ToString();
        }

        public static GossipNodeConfiguration Create(Action<Builder> configure)
        {
            var builder = new Builder();
            configure(builder);
            return new GossipNodeConfiguration(builder);
        }

        public class Builder
        {
            public Builder()
            {
                var encoderDecoder = new GossipMessageCodec();
                Logger = new LoggerConfiguration().CreateLogger();
            }

            public IPEndPoint LocalEndPoint { get; set; }

            public ILogger Logger { get; set; }

            public byte[] Metadata { get; set; }

            public string Name { get; set; }
        }
    }
}