using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace GossipNet.Swim
{
    public class SwimConfiguration
    {
        public SwimConfiguration(Builder builder)
        {
            Debug.Assert(builder != null);

            GossipFrequency = builder.GossipFrequency ?? TimeSpan.FromSeconds(2);
            GossipMemberSelector = builder.GossipMemberSelector ?? new RandomKGossipMemberSelector(2);
            if((LocalMember = builder.LocalMember) == null)
            {
                var localEndPoint = new IPEndPoint(IPAddress.Loopback, 23013);
                LocalMember = builder.LocalMember ?? new SwimMember(localEndPoint.ToString(), localEndPoint);
            }
            Logger = (builder.LoggerConfiguration ?? new LoggerConfiguration())
                .Destructure.AsScalar<IPEndPoint>()
                .CreateLogger();
            RetransmitCountCalculator = builder.RetransmitCountCalculator ?? (memberCount => (int)Math.Log(memberCount, 10d));
        }

        public TimeSpan GossipFrequency { get; private set; }

        public IGossipMemberSelector GossipMemberSelector { get; private set; }

        public SwimMember LocalMember { get; private set; }

        public ILogger Logger { get; private set; }

        public Func<int, int> RetransmitCountCalculator { get; private set; }

        public class Builder
        {
            /// <summary>
            /// Gets or sets the frequency to gossip with other members.
            /// </summary>
            public TimeSpan? GossipFrequency { get; set; }

            /// <summary>
            /// Gets or sets the gossip member selector. This is responsible for
            /// selecting the members to gossip with each round and is responsible
            /// for applying the `k` value in the SWIM paper.
            /// </summary>
            public IGossipMemberSelector GossipMemberSelector { get; set; }

            public SwimMember LocalMember { get; set; }

            public LoggerConfiguration LoggerConfiguration { get; set; }

            public Func<int, int> RetransmitCountCalculator { get; set; }
        }
    }
}