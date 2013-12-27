using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Messages
{
    public class GossipMessageCodec : IGossipMessageEncoder, IGossipMessageDecoder
    {
        public IEnumerable<GossipMessage> Decode(Stream stream)
        {
            using(var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                return Decode(reader);
            }
        }

        public void Encode(GossipMessage message, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write((byte)message.Type);
                switch (message.Type)
                {
                    case GossipMessageType.Compound:
                        EncodeCompound((CompoundMessage)message, writer);
                        break;
                    case GossipMessageType.Compressed:
                        EncodeCompressed((CompressedMessage)message, writer);
                        break;
                    case GossipMessageType.Ack:
                        EncodeAck((AckMessage)message, writer);
                        break;
                    case GossipMessageType.Alive:
                        EncodeAlive((AliveMessage)message, writer);
                        break;
                    case GossipMessageType.Dead:
                        EncodeDead((DeadMessage)message, writer);
                        break;
                    case GossipMessageType.Ping:
                        EncodePing((PingMessage)message, writer);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private IEnumerable<GossipMessage> Decode(BinaryReader reader)
        {
            // first byte has message type
            var messageType = (GossipMessageType)reader.ReadByte();

            switch (messageType)
            {
                case GossipMessageType.Compound:
                    return DecodeCompound(reader);
                case GossipMessageType.Compressed:
                    return DecodeCompressed(reader);
                case GossipMessageType.Ack:
                    return new[] { DecodeAck(reader) };
                case GossipMessageType.Alive:
                    return new[] { DecodeAlive(reader) };
                case GossipMessageType.Dead:
                    return new[] { DecodeDead(reader) };
                case GossipMessageType.Ping:
                    return new[] { DecodePing(reader) };
                default:
                    throw new NotSupportedException();
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

        private IEnumerable<GossipMessage> DecodeCompound(BinaryReader reader)
        {
            var messages = new List<GossipMessage>();
            var numMessages = reader.ReadInt32();
            for(int i = 0; i < numMessages; i++)
            {
                // TODO: read each message length?
                messages.AddRange(Decode(reader));
            }

            return messages;
        }

        private IEnumerable<GossipMessage> DecodeCompressed(BinaryReader reader)
        {
            var compressionType = (CompressionType)reader.ReadByte();
            switch(compressionType)
            {
                case CompressionType.Gzip:
                    using(var gzipStream = new GZipStream(reader.BaseStream, CompressionMode.Decompress, true))
                    {
                        return Decode(gzipStream);
                    }
                case CompressionType.Deflate:
                    using(var deflateStream = new DeflateStream(reader.BaseStream, CompressionMode.Decompress, true))
                    {
                        return Decode(deflateStream);
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        private DeadMessage DecodeDead(BinaryReader reader)
        {
            var name = reader.ReadString();
            var incarnation = reader.ReadInt32();
            return new DeadMessage(name, incarnation);
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

        private void EncodeCompound(CompoundMessage message, BinaryWriter writer)
        {
            writer.Write(message.EncodedMessages.Count);
            foreach(var msg in message.EncodedMessages)
            {
                // TODO: write each message length?
                writer.Write(msg);
            }
        }

        private void EncodeCompressed(CompressedMessage compressedMessage, BinaryWriter writer)
        {
            writer.Write((byte)compressedMessage.CompressionType);
            switch(compressedMessage.CompressionType)
            {
                case CompressionType.Gzip:
                    using(var gzipStream = new GZipStream(writer.BaseStream, CompressionLevel.Optimal, true))
                    {
                        Encode(compressedMessage.Message, gzipStream);
                    }
                    break;
                case CompressionType.Deflate:
                    using (var deflateStream = new DeflateStream(writer.BaseStream, CompressionLevel.Optimal, true))
                    {
                        Encode(compressedMessage.Message, deflateStream);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void EncodeDead(DeadMessage message, BinaryWriter writer)
        {
            writer.Write(message.Name);
            writer.Write(message.Incarnation);
        }

        private void EncodePing(PingMessage message, BinaryWriter writer)
        {
            writer.Write(message.SequenceNumber);
        }
    }
}