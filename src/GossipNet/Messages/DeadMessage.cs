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

        public override GossipMessageType MessageType
        {
            get { return GossipMessageType.Dead; }
        }

        public string Name { get; private set; }

        public override bool Invalidates(BroadcastableMessage other)
        {
            throw new NotImplementedException();
        }
    }
}