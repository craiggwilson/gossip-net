using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim.Messages
{
    public class CompositeMessage : SwimMessage
    {
        public CompositeMessage(IEnumerable<byte[]> rawMessages)
        {
            Debug.Assert(rawMessages != null);

            RawMessages = rawMessages.ToList().AsReadOnly();
        }

        public IReadOnlyList<byte[]> RawMessages { get; private set; }

        public override SwimMessageType Type
        {
            get { return SwimMessageType.Composite; }
        }
    }
}
