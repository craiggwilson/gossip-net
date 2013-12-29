using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Core;

namespace GossipNet.Swim
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMember"></typeparam>
    /// <remarks>http://www.cs.cornell.edu/~asdas/research/dsn02-swim.pdf</remarks>
    public class SwimMembershipProtocol<TMember> : IMembershipProtocol<SwimMessage, TMember> 
        where TMember : Member
    {
        private readonly SwimConfiguration<TMember> _config;

        public SwimMembershipProtocol(SwimConfiguration<TMember> config)
        {
            Debug.Assert(config != null);

            _config = config;
        }

        public event Action<TMember> MemberJoined;

        public event Action<TMember> MemberLeft;

        public IReadOnlyList<TMember> Members
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {

        }

        public void Disseminate(SwimMessage message)
        {
            throw new NotImplementedException();
        }

        public void JoinCluster()
        {
            throw new NotImplementedException();
        }

        public void LeaveCluster(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}