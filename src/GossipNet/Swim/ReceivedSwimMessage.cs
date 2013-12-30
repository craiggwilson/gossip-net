using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    public class ReceivedSwimMessage
    {
        public ReceivedSwimMessage(SwimMessage message, IPEndPoint remoteEndPoint)
        {
            Debug.Assert(remoteEndPoint != null);
            Debug.Assert(message != null);

            Message = message;
            RemoteEndPoint = remoteEndPoint;
        }

        public SwimMessage Message { get; private set; }

        public IPEndPoint RemoteEndPoint { get; private set; }
    }
}
