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

        public IGossipMessageDecoder MessageDecoder { get; private set; }

        public IGossipMessageEncoder MessageEncoder { get; private set; }

        public string Name { get; private set; }

        public TimeSpan ProbeFrequency { get; private set; }

        private GossipNodeConfiguration(Builder builder)
        {
            LocalEndPoint = builder.LocalEndPoint;
            Logger = builder.Logger;
            MessageDecoder = builder.MessageDecoder;
            MessageEncoder = builder.MessageEncoder;
            Name = builder.Name ?? LocalEndPoint.ToString();
            ProbeFrequency = builder.ProbeFrequency;
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
                var encoderDecoder = new GossipMessageEncoderDecoder();
                LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 5000);
                Logger = new LoggerConfiguration().CreateLogger();
                MessageDecoder = encoderDecoder;
                MessageEncoder = encoderDecoder;
                ProbeFrequency = TimeSpan.FromSeconds(2);
            }

            public IPEndPoint LocalEndPoint { get; set; }

            public ILogger Logger { get; set; }

            public IGossipMessageDecoder MessageDecoder { get; set; }

            public IGossipMessageEncoder MessageEncoder { get; set; }

            public string Name { get; set; }

            public TimeSpan ProbeFrequency { get; set; }
        }
    }
}