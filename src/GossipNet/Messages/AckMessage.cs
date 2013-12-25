using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class AckMessage : GossipMessage
    {
        public AckMessage(uint sequenceNumber)
        {
            SequenceNumber = sequenceNumber;
        }

        public override GossipMessageType MessageType
        {
            get { return GossipMessageType.Ack; }
        }

        public uint SequenceNumber { get; private set; }
    }
}