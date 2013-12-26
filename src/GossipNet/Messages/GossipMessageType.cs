﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public enum GossipMessageType : byte
    {
        Raw,
        Ping,
        Ack,
        Alive,
        Suspect,
        Dead,
    }
}   