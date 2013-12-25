using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public interface IGossipMessageEncoder
    {
        void Encode(GossipMessage message, Stream stream);
    }
}
