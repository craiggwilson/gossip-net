using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    internal class SwimBroadcast
    {
        public SwimBroadcast(SwimMessage message, byte[] messageBytes, EventWaitHandle @event)
        {
            Message = message;
            MessageBytes = messageBytes;
            Event = @event;
        }

        public SwimMessage Message { get; private set; }

        public byte[] MessageBytes { get; private set; }

        public EventWaitHandle Event { get; private set; }
    }
}