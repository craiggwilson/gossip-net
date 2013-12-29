using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Core;

namespace GossipNet.Swim
{
    public interface IGossipMemberSelector<TMember> 
        where TMember : Member
    {
        IEnumerable<TMember> Select(IReadOnlyList<TMember> members);
    }
}