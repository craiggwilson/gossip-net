using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Support;

namespace GossipNet.Swim
{
    internal class SwimBroadcastQueue
    {
        private readonly object _queueLock = new object();
        private readonly PriorityQueue<int, TransmittedBroadcast> _queue;
        private readonly Func<int> _retransmitCount;

        public SwimBroadcastQueue(Func<int> retransmitCount)
        {
            _queue = new PriorityQueue<int, TransmittedBroadcast>();
            _retransmitCount = retransmitCount;
        }

        public IEnumerable<SwimBroadcast> GetBroadcasts(int maxBytes)
        {
            var broadcasts = new List<SwimBroadcast>();
            var itemsForRetransmit = new List<TransmittedBroadcast>();
            var retransmitCount = _retransmitCount();
            lock (_queueLock)
            {
                int accumulatedBytes = 0;
                while (_queue.HasItems)
                {
                    if(accumulatedBytes + _queue.Peek().Broadcast.MessageBytes.Length < maxBytes)
                    {
                        var item = _queue.Dequeue();
                        broadcasts.Add(item.Broadcast);
                        accumulatedBytes += item.Broadcast.MessageBytes.Length;
                        if(item.RetransmitCount < retransmitCount)
                        {
                            itemsForRetransmit.Add(item);
                        }
                    }
                }

                // queue back up items that should be retransmitted
                foreach(var itemForRetransmit in itemsForRetransmit)
                {
                    itemForRetransmit.RetransmitCount++;
                    _queue.Enqueue(itemForRetransmit.RetransmitCount, itemForRetransmit);
                }
            }

            return broadcasts;
        }

        public void Enqueue(SwimBroadcast broadcast)
        {
            var tracked = new TransmittedBroadcast
            {
                Broadcast = broadcast,
                RetransmitCount = 0
            };

            lock (_queueLock)
            {
                _queue.Enqueue(0, tracked);
                // TODO: need to run through all the messages and remove them
                // if this one obsoletes them
            }
        }

        private class TransmittedBroadcast
        {
            public SwimBroadcast Broadcast;
            public int RetransmitCount;
        }
    }
}