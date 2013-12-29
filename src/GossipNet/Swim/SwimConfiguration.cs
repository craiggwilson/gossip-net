using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Core;

namespace GossipNet.Swim
{
    public class SwimConfiguration<TMember>
        where TMember : Member
    {
        public SwimConfiguration(Builder builder)
        {
            Debug.Assert(builder != null);
            Debug.Assert(builder.MemberCodec != null, "The member codec cannot be null.");

            GossipFrequency = builder.GossipFrequency ?? TimeSpan.FromSeconds(2);
            GossipMemberSelector = builder.GossipMemberSelector ?? new RandomKGossipMemberSelector<TMember>(2);
            MemberCodec = builder.MemberCodec;
        }

        public TimeSpan GossipFrequency { get; private set; }

        public IGossipMemberSelector<TMember> GossipMemberSelector { get; private set; }

        public IMemberCodec<TMember> MemberCodec { get; private set; }

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
            public IGossipMemberSelector<TMember> GossipMemberSelector { get; set; }

            public IMemberCodec<TMember> MemberCodec { get; set; }
        }
    }
}