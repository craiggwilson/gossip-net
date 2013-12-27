using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Messages;
using Serilog;

namespace GossipNet.IO
{
    public class GossipMessagePump : IGossipMessageReceiver, IGossipMessageSender, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IGossipMessageDecoder _messageDecoder;
        private readonly IGossipMessageEncoder _messageEncoder;
        private BroadcastQueue _broadcasts;
        private GossipUdpClient _client;
        private IPEndPoint _localEndPoint;

        public GossipMessagePump(IPEndPoint localEndPoint, IGossipMessageDecoder messageDecoder, IGossipMessageEncoder messageEncoder, ILogger logger)
        {
            _localEndPoint = localEndPoint;
            _messageDecoder = messageDecoder;
            _messageEncoder = messageEncoder;
            _logger = logger;
        }

        public event Action<IPEndPoint, GossipMessage> MessageReceived;

        public void Broadcast(BroadcastableMessage message)
        {
            byte[] messageBytes;
            using (var ms = new MemoryStream())
            {
                _messageEncoder.Encode(message, ms);
                messageBytes = ms.ToArray();
            }

            var broadcast = new Broadcast(message, messageBytes);
            _broadcasts.Enqueue(broadcast);
        }

        public void Dispose()
        {
            _client.Close();
        }

        public void Open(Func<int> broadcastTransmitCount)
        {
            _broadcasts = new BroadcastQueue(broadcastTransmitCount);
            _client = new GossipUdpClient(_localEndPoint, DatagramReceived, _logger);
        }

        public void Send(IPEndPoint remoteEndPoint, GossipMessage message)
        {
            // perhaps compression should happen here...
            using (var ms = new MemoryStream())
            {
                try
                {
                    _logger.Verbose("Sending {@Message} to {RemoteEndPoint}", message, remoteEndPoint);
                    _messageEncoder.Encode(message, ms);

                    List<byte[]> messageBytes = null;
                    foreach (var broadcast in _broadcasts.GetBroadcasts(0, 4096))
                    {
                        _logger.Verbose("Piggybacking {@Message} to {RemoteEndPoint}", broadcast.Message, remoteEndPoint);
                        if (messageBytes == null)
                        {
                            messageBytes = new List<byte[]>();
                            messageBytes.Add(ms.ToArray());
                            ms.SetLength(0);
                        }

                        messageBytes.Add(broadcast.MessageBytes);
                    }

                    if (messageBytes != null)
                    {
                        message = new CompoundMessage(messageBytes);
                        ms.SetLength(0);
                        _messageEncoder.Encode(message, ms);
                    }

                    if(true) // UseCompression
                    {
                        message = new CompressedMessage(CompressionType.Deflate, message);
                        ms.SetLength(0);
                        _messageEncoder.Encode(message, ms);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unable to encode {@Message} for {RemoteEndPoint}.", message, remoteEndPoint);
                    return;
                }

                _client.Send(remoteEndPoint, ms.ToArray());
            }
        }

        private void DatagramReceived(IPEndPoint remoteEndPoint, byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                try
                {
                    foreach (var message in _messageDecoder.Decode(ms))
                    {
                        if (MessageReceived != null)
                        {
                            try
                            {
                                MessageReceived(remoteEndPoint, message);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex, "Error occured in MessageReceived handler for {Message} from {RemoteEndPoint}.", message, remoteEndPoint);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unable to decode message from {RemoteEndPoint}.", remoteEndPoint);
                    return;
                }
            }
        }

        private class GossipUdpClient
        {
            private readonly UdpClient _client;
            private readonly ILogger _logger;
            private readonly Action<IPEndPoint, byte[]> _onReceive;
            private bool _closed;

            public GossipUdpClient(IPEndPoint localEndPoint, Action<IPEndPoint, byte[]> onReceive, ILogger logger)
            {
                Debug.Assert(localEndPoint != null);
                Debug.Assert(onReceive != null);
                Debug.Assert(logger != null);

                _client = new UdpClient();
                _client.ExclusiveAddressUse = false;
                _client.Client.Bind(localEndPoint);

                _onReceive = onReceive;
                _logger = logger;
                _client.BeginReceive(Receive, null);
            }

            public void Close()
            {
                _closed = true;
                _client.Close();
            }

            public void Send(IPEndPoint remoteEndPoint, byte[] bytes)
            {
                _client.Send(bytes, bytes.Length, remoteEndPoint);
            }

            private void Receive(IAsyncResult ar)
            {
                IPEndPoint remoteEndPoint = null;
                byte[] data = null;
                try
                {
                    data = _client.EndReceive(ar, ref remoteEndPoint);
                }
                catch (ObjectDisposedException)
                {
                    // thrown when the client is closed...
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Error occured in udp client EndReceive. Ignoring and moving on.");
                }

                if (!_closed)
                {
                    _client.BeginReceive(Receive, null);
                    if (remoteEndPoint != null && data != null)
                    {
                        _onReceive(remoteEndPoint, data);
                    }
                }
            }
        }
    }
}