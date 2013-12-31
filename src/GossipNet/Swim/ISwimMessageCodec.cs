using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Swim
{
    internal interface ISwimMessageCodec
    {
        int CompositeMessageOverheadInBytes { get; }

        int CompositeOverheadPerMessageInBytes { get; }

        IEnumerable<SwimMessage> Decode(Stream stream);

        void Encode(SwimMessage message, Stream stream);
    }
}