using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Core;

namespace GossipNet.Swim.Messages
{
    public class AliveMessage<TMember> : SwimMessage
        where TMember : Member
    {
        public AliveMessage(TMember member, int incarnationNumber)
        {
            Debug.Assert(member != null);

            Member = member;
            IncarnationNumber = incarnationNumber;
        }

        public int IncarnationNumber { get; private set; }

        public TMember Member { get; private set; }

        public override SwimMessageType Type
        {
            get { return SwimMessageType.Alive; }
        }
    }
}
