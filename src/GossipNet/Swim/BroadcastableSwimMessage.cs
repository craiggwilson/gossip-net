using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    public abstract class BroadcastableSwimMessage : SwimMessage
    {
        public virtual bool Overrides(BroadcastableSwimMessage other)
        {
            return false;
        }
    }
}