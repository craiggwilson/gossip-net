using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public enum GossipMessageType : byte
    {
        Compound = 1,
        Compressed,
        Encrypted,

        Ping = 10,
        Ack,
        Alive,
        Suspect,
        Dead,
    }
}   