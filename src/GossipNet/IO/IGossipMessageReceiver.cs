using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Messages;

namespace GossipNet.IO
{
    public interface IGossipMessageReceiver
    {
        event Action<IPEndPoint, GossipMessage> MessageReceived;
    }
}