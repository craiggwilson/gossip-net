using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet
{
    public class GossipNode
    {
        public GossipNode(string name, IPEndPoint ipEndPoint, byte[] metadata)
        {
            Name = name;
            IPEndPoint = ipEndPoint;
            Metadata = metadata;
        }

        public IPEndPoint IPEndPoint { get; private set; }

        public byte[] Metadata { get; private set; }

        public string Name { get; private set; }
    }
}