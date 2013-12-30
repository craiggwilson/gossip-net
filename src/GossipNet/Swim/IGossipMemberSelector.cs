using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Swim;

namespace GossipNet.Swim
{
    public interface IGossipMemberSelector
    {
        IEnumerable<SwimMember> Select(IReadOnlyList<SwimMember> members);
    }
}