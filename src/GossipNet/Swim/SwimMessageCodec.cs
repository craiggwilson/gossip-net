using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Core;
using GossipNet.Swim.Messages;

namespace GossipNet.Swim
{
    internal class SwimMessageCodec<TMember> : IMessageCodec
        where TMember: Member
    {
        private readonly IMemberCodec<TMember> _memberCodec;

        public SwimMessageCodec(IMemberCodec<TMember> memberCodec)
        {
            Debug.Assert(memberCodec != null);

            _memberCodec = memberCodec;
        }

        public IEnumerable<SwimMessage> Decode(Stream stream)
        {
            using(var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var type = (SwimMessageType)reader.ReadByte();
                switch(type)
                {
                    case SwimMessageType.Ack:
                        return DecodeAck(reader).ToEnumerable();
                    case SwimMessageType.Alive:
                        return DecodeAlive(reader).ToEnumerable();
                    case SwimMessageType.Ping:
                        return DecodePing(reader).ToEnumerable();
                    case SwimMessageType.PingRequest:
                        return DecodePingRequest(reader).ToEnumerable();
                    default:
                        throw new NotSupportedException(string.Format("SwimMessageType with value {0} is a support message type.", (int)type));
                }
            }
        }

        public void Encode(SwimMessage message, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write((byte)message.Type);
                switch (message.Type)
                {
                    case SwimMessageType.Ack:
                        EncodeAck((AckMessage)message, writer);
                        break;
                    case SwimMessageType.Alive:
                        EncodeAlive((AliveMessage<TMember>)message, writer);
                        break;
                    case SwimMessageType.Ping:
                        EncodePing((PingMessage)message, writer);
                        break;
                    case SwimMessageType.PingRequest:
                        EncodePingReq((PingRequestMessage)message, writer);
                        break;
                }
            }
        }

        private SwimMessage DecodeAck(BinaryReader reader)
        {
            return new AckMessage(reader.ReadInt32());
        }

        private SwimMessage DecodeAlive(BinaryReader reader)
        {
            return new AliveMessage<TMember>(
                _memberCodec.Decode(reader),
                reader.ReadInt32());
        }

        private SwimMessage DecodePing(BinaryReader reader)
        {
            return new PingMessage(reader.ReadInt32());
        }

        private SwimMessage DecodePingRequest(BinaryReader reader)
        {
            return new PingRequestMessage(
                reader.ReadInt32(),
                reader.ReadIPEndPoint());
        }

        private void EncodeAck(AckMessage message, BinaryWriter writer)
        {
            writer.Write(message.SequenceNumber);
        }

        private void EncodeAlive(AliveMessage<TMember> message, BinaryWriter writer)
        {
            _memberCodec.Encode(message.Member, writer);
            writer.Write(message.IncarnationNumber);
        }

        private void EncodePing(PingMessage message, BinaryWriter writer)
        {
            writer.Write(message.SequenceNumber);
        }

        private void EncodePingReq(PingRequestMessage message, BinaryWriter writer)
        {
            writer.Write(message.SequenceNumber);
            writer.Write(message.EndPoint);
        }
    }
}