using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    internal interface IMessageCodec
    {
        IEnumerable<SwimMessage> Decode(Stream stream);

        void Encode(SwimMessage message, Stream stream);
    }
}