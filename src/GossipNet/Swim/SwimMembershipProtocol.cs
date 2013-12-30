using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMember"></typeparam>
    /// <remarks>http://www.cs.cornell.edu/~asdas/research/dsn02-swim.pdf</remarks>
    public class SwimMembershipProtocol
    {
        private readonly SwimConfiguration _config;

        public SwimMembershipProtocol(SwimConfiguration config)
        {
            Debug.Assert(config != null);

            _config = config;
        }

        public event Action<SwimMember> MemberJoined;

        public event Action<SwimMember> MemberLeft;

        public IReadOnlyList<SwimMember> Members
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