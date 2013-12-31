using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    public enum SwimMessageType : byte
    {
        Composite,
        Compressed,

        Ping,
        Ack,
        PingRequest,
        Alive,
        Suspect,
        Dead
    }
}