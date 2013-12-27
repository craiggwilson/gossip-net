using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class CompressedMessage : GossipMessage
    {
        public CompressedMessage(CompressionType compressionType, GossipMessage message)
        {
            Message = message;
            CompressionType = compressionType;
        }

        public CompressionType CompressionType { get; private set; }

        public GossipMessage Message { get; private set; }

        public override GossipMessageType Type
        {
            get { return GossipMessageType.Compressed; }
        }
    }
}