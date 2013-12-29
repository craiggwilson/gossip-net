using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Core
{
    public class Member
    {
        public Member(string name, IPEndPoint endPoint)
        {
            Debug.Assert(name != null);
            Debug.Assert(endPoint != null);

            Name = name;
            EndPoint = endPoint;
        }

        public string Name { get; private set; }

        public IPEndPoint EndPoint { get; private set; }
    }
}