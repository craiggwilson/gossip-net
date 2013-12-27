using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class AliveMessage : BroadcastableMessage
    {
        public AliveMessage(string name,
            IPEndPoint ipEndPoint,
            byte[] metadata,
            int incarnation)
        {
            Name = name;
            IPEndPoint = ipEndPoint;
            Metadata = metadata;
            Incarnation = incarnation;
        }

        public int Incarnation { get; private set; }

        public IPEndPoint IPEndPoint { get; private set; }

        public byte[] Metadata { get; private set; }

        public string Name { get; private set; }

        public override GossipMessageType Type
        {
            get { return GossipMessageType.Alive; }
        }

        public override bool Invalidates(BroadcastableMessage other)
        {
            switch(other.Type)
            {
                case GossipMessageType.Alive:
                    return ((AliveMessage)other).Name == Name;
                case GossipMessageType.Dead:
                    return ((DeadMessage)other).Name == Name;
                default:
                    return false;
            }
        }
    }
}