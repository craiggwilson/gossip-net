using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class DeadMessage : BroadcastableMessage
    {
        public DeadMessage(string name, int incarnation)
        {
            Name = name;
            Incarnation = incarnation;
        }

        public int Incarnation { get; private set; }

        public string Name { get; private set; }

        public override GossipMessageType Type
        {
            get { return GossipMessageType.Dead; }
        }

        public override bool Invalidates(BroadcastableMessage other)
        {
            switch (other.Type)
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