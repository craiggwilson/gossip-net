using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class GossipMessageCodec : IGossipMessageEncoder, IGossipMessageDecoder
    {
        public GossipMessage Decode(Stream stream)
        {
            using(var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                // first byte has message type
                var messageType = (GossipMessageType)stream.ReadByte();

                switch(messageType)
                {
                    case GossipMessageType.Ack:
                        return DecodeAck(reader);
                    case GossipMessageType.Alive:
                        return DecodeAlive(reader);
                    case GossipMessageType.Ping:
                        return DecodePing(reader);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public void Encode(GossipMessage message, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                if(message.MessageType == GossipMessageType.Raw)
                {
                    writer.Write(((RawMessage)message).Bytes);
                    return;
                }

                writer.Write((byte)message.MessageType);
                switch (message.MessageType)
                {
                    case GossipMessageType.Ack:
                        EncodeAck((AckMessage)message, writer);
                        break;
                    case GossipMessageType.Alive:
                        EncodeAlive((AliveMessage)message, writer);
                        break;
                    case GossipMessageType.Ping:
                        EncodePing((PingMessage)message, writer);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private AckMessage DecodeAck(BinaryReader reader)
        {
            var sequenceNumber = reader.ReadInt32();
            return new AckMessage(sequenceNumber);
        }

        private AliveMessage DecodeAlive(BinaryReader reader)
        {
            var name = reader.ReadString();
            var ipAddressLength = reader.ReadInt32();
            var ipAddress = new IPAddress(reader.ReadBytes(ipAddressLength));
            var port = reader.ReadInt32();
            var ipEndPoint = new IPEndPoint(ipAddress, port);
            var metaLength = reader.ReadInt32();
            byte[] meta = null;
            if (metaLength > 0)
            {
                meta = reader.ReadBytes(metaLength);
            }
            var incarnation = reader.ReadInt32();

            return new AliveMessage(name, ipEndPoint, meta, incarnation);
        }

        private PingMessage DecodePing(BinaryReader reader)
        {
            var sequenceNumber = reader.ReadInt32();
            return new PingMessage(sequenceNumber);
        }

        private void EncodeAck(AckMessage message, BinaryWriter writer)
        {
            writer.Write(message.SequenceNumber);
        }

        private void EncodeAlive(AliveMessage message, BinaryWriter writer)
        {
            writer.Write(message.Name);
            var ipAddressBytes = message.IPEndPoint.Address.GetAddressBytes();
            writer.Write(ipAddressBytes.Length);
            writer.Write(ipAddressBytes);
            writer.Write(message.IPEndPoint.Port);
            writer.Write(message.Metadata == null ? 0 : message.Metadata.Length);
            if(message.Metadata != null && message.Metadata.Length > 0)
            {
                writer.Write(message.Metadata);
            }
            writer.Write(message.Incarnation);
        }

        private void EncodePing(PingMessage message, BinaryWriter writer)
        {
            writer.Write(message.SequenceNumber);
        }
    }
}