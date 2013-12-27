using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class CompoundMessage : GossipMessage
    {
        public CompoundMessage(IEnumerable<byte[]> bytes)
        {
            Messages = bytes.ToList().AsReadOnly();
        }

        public override GossipMessageType MessageType
        {
            get { return GossipMessageType.Compound; }
        }

        public IReadOnlyList<byte[]> Messages { get; private set; }
    }
}