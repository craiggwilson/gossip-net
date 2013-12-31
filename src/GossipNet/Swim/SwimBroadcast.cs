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
        public SwimBroadcast(SwimMessage message, byte[] rawMessage, EventWaitHandle waitHandle)
        {
            Message = message;
            RawMessage = rawMessage;
            Event = waitHandle;
        }

        public EventWaitHandle Event { get; private set; }

        public SwimMessage Message { get; private set; }

        public byte[] RawMessage { get; private set; }
    }
}