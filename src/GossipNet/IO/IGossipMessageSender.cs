using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Messages;

namespace GossipNet.IO
{
    public interface IGossipMessageSender
    {
        void Broadcast(BroadcastableMessage message);

        void Send(IPEndPoint remoteEndPoint, GossipMessage message);
    }
}