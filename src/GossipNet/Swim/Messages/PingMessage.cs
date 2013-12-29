using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim.Messages
{
    public class PingMessage : SwimMessage
    {
        public PingMessage(int sequenceNumber)
        {
            SequenceNumber = sequenceNumber;
        }

        public int SequenceNumber { get; private set; }

        public override SwimMessageType Type
        {
            get { return SwimMessageType.Ping; }
        }
    }
}
