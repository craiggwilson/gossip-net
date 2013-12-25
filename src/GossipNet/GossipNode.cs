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
using GossipNet.Messages;

namespace GossipNet
{
    public class GossipNode : IDisposable
    {
        private readonly object _broadcastQueueLock = new object();
        private readonly object _nodeCollectionLock = new object();
        private readonly object _probeLock = new object();
        private readonly GossipNodeConfiguration _configuration;
        private readonly Timer _probeTimer;
        private readonly GossipUdpClient _udpClient;

        private readonly GossipNodeCollection _nodeCollection;
        private readonly BroadcastQueue _broadcastQueue;

        private int _incarnationNumber;
        private int _sequenceNumber;

        public GossipNode(GossipNodeConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            _configuration = configuration;
            _udpClient = new GossipUdpClient(configuration.LocalEndPoint, HandleUdpPacket);
            _nodeCollection = new GossipNodeCollection();
            _broadcastQueue = new BroadcastQueue(() => (uint)Math.Log(Nodes.Count), _configuration.MessageEncoder);

            SetAlive(); // this adds us into the node collection

            _udpClient.Open();
            _probeTimer = new Timer(_ => Probe(), null, _configuration.ProbeFrequency, TimeSpan.Zero);
            _configuration.Logger.Information("Started at {LocalEndPoint}", _configuration.LocalEndPoint);
        }

        public IReadOnlyList<GossipNodeDescription> Nodes
        {
            get
            {
                lock(_nodeCollectionLock)
                {
                    return _nodeCollection.AsReadOnly();
                }
            }
        }

        public void Join(IPEndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null) throw new ArgumentNullException("remoteEndPoint");

            _configuration.Logger.Information("Joining cluster with {RemoteEndPoint}", remoteEndPoint);
            
            var ping = new PingMessage(GetNextSequenceNumber());
            SendUdpMessage(remoteEndPoint, ping);
        }

        public void Dispose()
        {
            _probeTimer.Dispose();
            _udpClient.Close();
        }

        private void EnqueueAndBroadcast(GossipMessage message)
        {
            lock(_broadcastQueueLock)
            {
                _broadcastQueue.Enqueue(message);
            }
        }

        private void ExchangeState()
        {

        }

        private uint GetNextIncarnationNumber()
        {
            return (uint)Interlocked.Increment(ref _incarnationNumber);
        }

        private uint GetNextSequenceNumber()
        {
            return (uint)Interlocked.Increment(ref _sequenceNumber);
        }

        private void HandleUdpPacket(IPEndPoint remoteEndPoint, byte[] bytes)
        {
            GossipMessage message;
            using(var ms = new MemoryStream(bytes))
            {
                message = _configuration.MessageDecoder.Decode(ms);
            }

            _configuration.Logger.Verbose("Received {@Message} from {RemoteEndPoint}", message, remoteEndPoint);
            switch(message.MessageType)
            {
                case GossipMessageType.Ping:
                    HandlePing(remoteEndPoint, (PingMessage)message);
                    break;
                case GossipMessageType.Ack:
                    HandleAck(remoteEndPoint, (AckMessage)message);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void HandleAck(IPEndPoint remoteEndPoint, AckMessage message)
        {
            // do nothing yet...
        }

        private void HandleAlive(IPEndPoint remoteEndPoint, AliveMessage message)
        {
            MarkNodeAsAlive(message);
        }

        private void HandlePing(IPEndPoint remoteEndPoint, PingMessage message)
        {
            var ack = new AckMessage(message.SequenceNumber);
            SendUdpMessage(remoteEndPoint, ack);
        }

        private void Probe()
        {
            var lockTaken = false;
            try
            {
                // don't wait... if we are already probing, then exit...
                lockTaken = Monitor.TryEnter(_probeLock, TimeSpan.Zero);
                if (!lockTaken) return;


                // probe logic goes here...

                                
                // TODO: probe frequency should be random to avoid synchronization
                _probeTimer.Change(_configuration.ProbeFrequency, TimeSpan.Zero);
            }
            finally
            {
                if(lockTaken)
                {
                    Monitor.Exit(_probeLock);
                }
            }
        }

        private void SendUdpMessage(IPEndPoint remoteEndPoint, GossipMessage message)
        {
            _configuration.Logger.Verbose("Sending {@Message} to {RemoteEndPoint}", message, remoteEndPoint);

            using(var ms = new MemoryStream())
            {
                _configuration.MessageEncoder.Encode(message, ms);

                var bytes = ms.ToArray();
                _udpClient.Send(remoteEndPoint, bytes);
            }
        }

        private void MarkNodeAsAlive(AliveMessage message)
        {
            GossipNodeDescription old;
            lock (_nodeCollectionLock)
            {
                old = _nodeCollection.GetOrAdd(message.Name, () =>
                {
                    return new GossipNodeDescription(message.Name,
                        message.IPEndPoint,
                        message.Meta,
                        message.Incarnation,
                        GossipNodeState.Dead,
                        DateTime.UtcNow);
                });
            }

            if (!old.IPEndPoint.Equals(message.IPEndPoint))
            {
                _configuration.Logger.Error("Conflicting endpoints for {Name}. Mine: {MyEndPoint} Theirs: {TheirEndPoint}", old.Name, old.IPEndPoint, message.IPEndPoint);
                return;
            }

            if (old.Incarnation >= message.Incarnation)
            {
                _configuration.Logger.Verbose("Received old incarnation alive request for {Name}. Mine: {MyIncarnation} Theirs: {TheirIncarnation}", old.Name, old.Incarnation, message.Incarnation);
                return;
            }

            EnqueueAndBroadcast(message);

            var @new = old.Update(GossipNodeState.Alive, message.Incarnation, DateTime.UtcNow);

            lock(_nodeCollectionLock)
            {
                _nodeCollection.Update(@new);
            }

            if (old.State == GossipNodeState.Dead)
            {
                // TODO: Raise event for join
            }
        }

        private void SetAlive()
        {
            // sets ourself into the alive state
            MarkNodeAsAlive(new AliveMessage(_configuration.Name,
                _configuration.LocalEndPoint,
                null,
                GetNextIncarnationNumber()));
        }
    }
}