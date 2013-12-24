using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet
{
    public class GossipNodeConfiguration
    {
        public IPEndPoint LocalEndPoint { get; private set; }

        private GossipNodeConfiguration(Builder builder)
        {
            LocalEndPoint = builder.LocalEndPoint;
        }

        public static GossipNodeConfiguration Create(Action<Builder> configure)
        {
            var builder = new Builder();
            configure(builder);
            return new GossipNodeConfiguration(builder);
        }

        public class Builder
        {
            public IPEndPoint LocalEndPoint { get; set; }
        }
    }
}