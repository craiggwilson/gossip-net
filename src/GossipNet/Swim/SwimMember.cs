using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    public class SwimMember
    {
        public SwimMember(string id, IPEndPoint endPoint)
        {
            Debug.Assert(id != null);
            Debug.Assert(endPoint != null);

            Id = id;
            EndPoint = endPoint;
        }

        public IPEndPoint EndPoint { get; private set; }

        public string Id { get; private set; }
    }
}