using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Core
{
    public class MemberCodec : AbstractMemberCodec<Member>
    {
        public override Member Decode(BinaryReader reader)
        {
            return new Member(
                ReadMemberName(reader),
                ReadIPEndPoint(reader));
        }

        public override void Encode(Member member, BinaryWriter writer)
        {
            WriteMemberName(writer, member.Name);
            WriteIPEndPoint(writer, member.EndPoint);
        }
    }
}