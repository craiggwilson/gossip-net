using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Messages;

namespace GossipNet.IO
{
    internal class Broadcast
    {
        public Broadcast(BroadcastableMessage message, byte[] messageBytes)
        {
            Message = message;
            MessageBytes = messageBytes;
        }

        public BroadcastableMessage Message { get; private set; }

        public byte[] MessageBytes { get; private set; }
    }
}