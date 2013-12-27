using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class CompoundMessage : GossipMessage
    {
        public CompoundMessage(IEnumerable<byte[]> encodedMessages)
        {
            EncodedMessages = encodedMessages.ToList().AsReadOnly();
        }

        public IReadOnlyList<byte[]> EncodedMessages { get; private set; }

        public override GossipMessageType Type
        {
            get { return GossipMessageType.Compound; }
        }
    }
}