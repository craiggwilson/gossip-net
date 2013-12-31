using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GossipNet.Swim.Messages;
using Serilog;

namespace GossipNet.Swim
{
    internal class UdpSwimMessageService : ISwimMessageService
    {
        private readonly SwimBroadcastQueue _broadcastQueue;
        private readonly Client _client;
        private readonly SwimConfiguration _config;
        private readonly ISwimMessageCodec _messageCodec;

        public UdpSwimMessageService(SwimConfiguration config, ISwimMessageCodec messageCodec, Func<int> retransmitCount)
        {
            Debug.Assert(config != null);
            Debug.Assert(messageCodec != null);
            Debug.Assert(retransmitCount != null);

            _config = config;
            _broadcastQueue = new SwimBroadcastQueue(retransmitCount);
            _client = new Client(config.LocalMember.EndPoint, DatagramReceived, config.Logger);
        }

        public event Action<ReceivedSwimMessage> MessageReceived;

        public void Dispose()
        {
            _client.Close();
        }

        public void QueueForBroadcast(BroadcastableSwimMessage message, EventWaitHandle waitHandle)
        {
            Debug.Assert(message != null);

            using (var ms = new MemoryStream())
            {
                _messageCodec.Encode(message, ms);
                _broadcastQueue.Enqueue(new SwimBroadcast(message, ms.ToArray(), waitHandle));
            }
        }

        public void Send(SwimMember member, SwimMessage message)
        {
            Debug.Assert(member != null);
            Debug.Assert(message != null);

            using (var ms = new MemoryStream())
            {
                _messageCodec.Encode(message, ms);

                var availableBytes = _config.MaximumGossipMessageSize - _messageCodec.CompositeMessageOverheadInBytes - (int)ms.Length;

                List<byte[]> rawMessages = null;
                foreach (var broadcast in _broadcastQueue.GetBroadcasts(_messageCodec.CompositeOverheadPerMessageInBytes, availableBytes))
                {
                    if (rawMessages == null)
                    {
                        rawMessages = new List<byte[]>();
                        rawMessages.Add(ms.ToArray());
                    }

                    rawMessages.Add(broadcast.RawMessage);
                }

                if (rawMessages != null)
                {
                    message = new CompositeMessage(rawMessages);
                    ms.SetLength(0);
                    _messageCodec.Encode(message, ms);
                }

                _client.Send(member.EndPoint, ms.ToArray());
            }
        }

        private void DatagramReceived(IPEndPoint remoteEndPoint, byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                try
                {
                    foreach (var message in _messageCodec.Decode(ms))
                    {
                        if (MessageReceived != null)
                        {
                            try
                            {
                                MessageReceived(new ReceivedSwimMessage(message, remoteEndPoint));
                            }
                            catch (Exception ex)
                            {
                                _config.Logger.Error(ex, "Error occured in MessageReceived handler for {Message} from {RemoteEndPoint}.", message, remoteEndPoint);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _config.Logger.Error(ex, "Unable to decode message from {RemoteEndPoint}.", remoteEndPoint);
                    return;
                }
            }
        }

        private class Client
        {
            private readonly UdpClient _client;
            private readonly ILogger _logger;
            private readonly Action<IPEndPoint, byte[]> _onReceive;
            private bool _closed;

            public Client(IPEndPoint localEndPoint, Action<IPEndPoint, byte[]> onReceive, ILogger logger)
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
                catch (SocketException ex)
                {
                    // thrown for seemingly no apparent reason :(
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