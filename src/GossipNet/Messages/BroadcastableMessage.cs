using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public abstract class BroadcastableMessage : GossipMessage
    {
        public abstract bool Invalidates(BroadcastableMessage other);
    }
}