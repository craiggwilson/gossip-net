using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Core
{
    public abstract class AbstractMemberCodec<TMember> : IMemberCodec<TMember>
        where TMember : Member
    {
        public abstract TMember Decode(BinaryReader reader);

        public abstract void Encode(TMember member, BinaryWriter writer);

        protected IPEndPoint ReadIPEndPoint(BinaryReader reader)
        {
            return reader.ReadIPEndPoint();
        }

        protected string ReadMemberName(BinaryReader reader)
        {
            return reader.ReadString();
        }

        protected void WriteIPEndPoint(BinaryWriter writer, IPEndPoint endPoint)
        {
            writer.Write(endPoint);
        }

        protected void WriteMemberName(BinaryWriter writer, string memberName)
        {
            writer.Write(memberName);
        }
    }
}