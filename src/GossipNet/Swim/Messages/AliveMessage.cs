using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim.Messages
{
    public class AliveMessage : BroadcastableSwimMessage
    {
        public AliveMessage(SwimMember member, int incarnationNumber)
        {
            Debug.Assert(member != null);

            Member = member;
            IncarnationNumber = incarnationNumber;
        }

        public int IncarnationNumber { get; private set; }

        public SwimMember Member { get; private set; }

        public override SwimMessageType Type
        {
            get { return SwimMessageType.Alive; }
        }

        public bool Overrides(BroadcastableSwimMessage other)
        {
            switch(other.Type)
            {
                case SwimMessageType.Alive:
                    var alive = (AliveMessage)other;
                    return Member.Id == alive.Member.Id 
                        && IncarnationNumber > alive.IncarnationNumber;
                //case SwimMessageType.Suspect:
                //    var suspect = (SuspectMessage)other);
                //    return Member.Name == suspect.MemberId
                //        && IncarnationNumber > suspect.IncarnationNumber;
                //case SwimMessageType.Dead:
                //    var dead = (DeadMessage)other);
                //    return Member.Name == dead.MemberId
                //        && IncarnationNumber > dead.IncarnationNumber;
            }

            return false;
        }
    }
}