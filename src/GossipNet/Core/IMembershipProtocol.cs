using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Core
{
    public interface IMembershipProtocol<TMessage, TMember> : IGossipProtocol<TMessage>
        where TMember : Member
    {
        event Action<TMember> MemberJoined;
        event Action<TMember> MemberLeft;

        IReadOnlyList<TMember> Members { get; }
    }
}