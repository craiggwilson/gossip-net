using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class AliveMessage : GossipMessage
    {
        public AliveMessage(string name,
            IPEndPoint ipEndPoint,
            byte[] meta,
            uint incarnation)
        {
            Name = name;
            IPEndPoint = ipEndPoint;
            Meta = meta;
            Incarnation = incarnation;
        }

        public uint Incarnation { get; private set; }

        public IPEndPoint IPEndPoint { get; private set; }

        public override GossipMessageType MessageType
        {
            get { return GossipMessageType.Alive; }
        }

        public byte[] Meta { get; private set; }

        public string Name { get; private set; }
    }
}
