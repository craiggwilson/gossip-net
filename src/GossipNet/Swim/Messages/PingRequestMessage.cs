using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim.Messages
{
    public class PingRequestMessage : SwimMessage
    {
        public PingRequestMessage(int sequenceNumber, IPEndPoint endPoint)
        {
            Debug.Assert(endPoint != null);

            SequenceNumber = sequenceNumber;
            EndPoint = endPoint;
        }

        public IPEndPoint EndPoint { get; private set; }

        public int SequenceNumber { get; private set; }

        public override SwimMessageType Type
        {
            get { return SwimMessageType.PingRequest; }
        }
    }
}
