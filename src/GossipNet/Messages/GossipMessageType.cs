using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public enum GossipMessageType : byte
    {
        Ping,
        Ack,
        Alive
    }
}   