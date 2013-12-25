using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet
{
    public class GossipNodeDescription
    {
        public GossipNodeDescription(string name, 
            IPEndPoint ipEndPoint, 
            byte[] metadata, 
            uint incarnation,
            GossipNodeState state,
            DateTime updatedAtUtc)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (ipEndPoint == null) throw new ArgumentNullException("ipEndPoint");

            Name = name;
            IPEndPoint = ipEndPoint;
            Metadata = metadata;

            Incarnation = incarnation;
            State = state;
            UpdatedAtUtc = updatedAtUtc;
        }

        public string Name { get; private set; }

        public IPEndPoint IPEndPoint { get; private set; }

        public byte[] Metadata { get; private set; }

        public uint Incarnation { get; private set; }

        public GossipNodeState State { get; private set; }

        public DateTime UpdatedAtUtc { get; private set; }

        public GossipNodeDescription Update(GossipNodeState state, uint incarnation, DateTime updatedAtUtc)
        {
            return new GossipNodeDescription(
                Name,
                IPEndPoint,
                Metadata,
                incarnation,
                state,
                updatedAtUtc);
        }
    }
}