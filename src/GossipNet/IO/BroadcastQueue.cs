using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Messages;
using GossipNet.Support;

namespace GossipNet.IO
{
    internal class BroadcastQueue
    {
        private readonly object _queueLock = new object();
        private readonly PriorityQueue<int, TrackedBroadcast> _queue;
        private readonly Func<int> _retransmitCount;

        public BroadcastQueue(Func<int> retransmitCount)
        {
            _queue = new PriorityQueue<int, TrackedBroadcast>();
            _retransmitCount = retransmitCount;
        }

        public IEnumerable<Broadcast> GetBroadcasts(int broadcastBytesOverhead, int maxBytes)
        {
            var broadcasts = new List<Broadcast>();
            var itemsForRetransmit = new List<TrackedBroadcast>();
            var retransmitCount = _retransmitCount();
            lock (_queueLock)
            {
                int accumulatedBytes = 0;
                while (_queue.HasItems)
                {
                    if(accumulatedBytes + _queue.Peek().Broadcast.MessageBytes.Length + broadcastBytesOverhead < maxBytes)
                    {
                        var item = _queue.Dequeue();
                        broadcasts.Add(item.Broadcast);
                        accumulatedBytes += item.Broadcast.MessageBytes.Length + broadcastBytesOverhead;
                        if(item.TransmitCount < retransmitCount)
                        {
                            itemsForRetransmit.Add(item);
                        }
                    }
                }

                // queue back up items that should be retransmitted
                foreach(var itemForRetransmit in itemsForRetransmit)
                {
                    itemForRetransmit.TransmitCount++;
                    _queue.Enqueue(itemForRetransmit.TransmitCount, itemForRetransmit);
                }
            }

            return broadcasts;
        }

        public void Enqueue(Broadcast broadcast)
        {
            var tracked = new TrackedBroadcast
            {
                Broadcast = broadcast,
                TransmitCount = 0
            };

            lock (_queueLock)
            {
                _queue.Enqueue(0, tracked);
            }
        }

        private class TrackedBroadcast
        {
            public Broadcast Broadcast;
            public int TransmitCount;
        }
    }
}