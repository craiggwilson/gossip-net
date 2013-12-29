using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Core
{
    public interface IMemberCodec<TMember> where TMember : Member
    {
        TMember Decode(BinaryReader reader);

        void Encode(TMember member, BinaryWriter writer);
    }
}