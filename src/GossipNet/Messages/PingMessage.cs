using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class PingMessage : GossipMessage
    {
        public PingMessage(uint sequenceNumber)
        {
            SequenceNumber = sequenceNumber;
        }

        public override GossipMessageType MessageType
        {
            get { return GossipMessageType.Ping; }
        }

        public uint SequenceNumber { get; private set; }
    }
}