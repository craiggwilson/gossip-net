using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class RawMessage : GossipMessage
    {
        public RawMessage(byte[] bytes)
        {
            Bytes = bytes;
        }

        public override GossipMessageType MessageType
        {
            get { return GossipMessageType.Raw; }
        }

        public byte[] Bytes { get; private set; }
    }
}
