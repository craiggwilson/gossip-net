using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Messages;

namespace GossipNet
{
    internal class BroadcastQueue
    {
        private readonly IGossipMessageEncoder _messageEncoder;
        private readonly List<Broadcast> _queue;
        private readonly Func<uint> _retransmitCount;

        public BroadcastQueue(Func<uint> retransmitCount, IGossipMessageEncoder messageEncoder)
        {
            _queue = new List<Broadcast>();
            _retransmitCount = retransmitCount;
            _messageEncoder = messageEncoder;
        }

        public void Enqueue(GossipMessage message)
        {
            using(var ms = new MemoryStream())
            {
                _messageEncoder.Encode(message, ms);

                var broadcast = new Broadcast { Message = message };
                broadcast.Bytes = ms.ToArray();

                _queue.Add(broadcast);

                // TODO: determine if any existing broadcasts are invalid
            }
        }

        private class Broadcast
        {
            public byte[] Bytes;
            public GossipMessage Message;
            public uint TransmitCount;
        }
    }
}