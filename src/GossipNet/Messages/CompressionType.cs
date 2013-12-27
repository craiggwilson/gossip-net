using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public enum CompressionType : byte
    {
        Deflate = 0,
        Gzip = 1
    }
}