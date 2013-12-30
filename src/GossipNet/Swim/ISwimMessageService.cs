using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    public interface ISwimMessageService : IDisposable
    {
        event Action<ReceivedSwimMessage> MessageReceived;

        void QueueForBroadcast(BroadcastableSwimMessage message);

        void Send(SwimMember member, SwimMessage message);
    }
}