using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GossipNet.Messages;

namespace GossipNet.IO
{
    internal class Broadcast
    {
        public Broadcast(BroadcastableMessage message, byte[] messageBytes, EventWaitHandle @event)
        {
            Message = message;
            MessageBytes = messageBytes;
            Event = @event;
        }

        public BroadcastableMessage Message { get; private set; }

        public byte[] MessageBytes { get; private set; }

        public EventWaitHandle Event { get; private set; }
    }
}