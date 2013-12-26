﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class PingMessage : GossipMessage
    {
        public PingMessage(int sequenceNumber)
        {
            SequenceNumber = sequenceNumber;
        }

        public override GossipMessageType MessageType
        {
            get { return GossipMessageType.Ping; }
        }

        public int SequenceNumber { get; private set; }
    }
}