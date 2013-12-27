using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GossipNet.IO;
using GossipNet.Messages;
using GossipNet.Support;

namespace GossipNet
{
    public class LocalGossipNode : IDisposable
    {
        private readonly object _broadcastLock = new object();
        private readonly object _lock = new object();
        private readonly Timer _broadcastTimer;
        private readonly GossipNodeConfiguration _configuration;
        private readonly GossipMessagePump _messagePump;
        private readonly DelegateKeyedCollection<string, TrackedGossipNode> _nodes;
        private readonly Random _random;

        private bool _disposed;
        private int _incarnationNumber;
        private ManualResetEvent _leaveEvent;
        private int _sequenceNumber;

        public LocalGossipNode(GossipNodeConfiguration configuration)
        {
            _configuration = configuration;

            _nodes = new DelegateKeyedCollection<string, TrackedGossipNode>(x => x.Name);
            _random = new Random();

            var codec = new GossipMessageCodec();
            _messagePump = new GossipMessagePump(configuration,
                codec,
                codec);

            _configuration.Logger.Information("Started {Name} at {LocalEndPoint}", configuration.Name, configuration.LocalEndPoint);

            _messagePump.MessageReceived += OnMessageReceived;
            _messagePump.Open(GetRetransmitCount);

            SetAlive();
            _broadcastTimer = new Timer(_ => Broadcast(), null, _configuration.BroadcastFrequency, _configuration.BroadcastFrequency);
        }

        public event Action<GossipNode> NodeJoined;

        public event Action<GossipNode> NodeLeft;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _configuration.Logger.Verbose("{Name} is shutting down", _configuration.Name);

                _broadcastTimer.Dispose();

                _messagePump.MessageReceived -= OnMessageReceived;
                _messagePump.Dispose();

                if(_leaveEvent != null)
                {
                    _leaveEvent.Dispose();
                }

                _configuration.Logger.Verbose("{Name} has shutdown", _configuration.Name);
            }
        }

        public void JoinCluster(IPEndPoint primary, params IPEndPoint[] other)
        {
            ThrowIfDisposed();

            // TODO: this should be a full state transfer, but for now it
            // is just a ping that will likely piggyback an alive message, and
            // each node will then have 2 nodes in their list.
            _messagePump.Send(primary, new PingMessage(GetNextSequenceNumber()));
        }

        public void LeaveCluster(TimeSpan timeout)
        {
            ThrowIfDisposed();

            _leaveEvent = new ManualResetEvent(false);
            HandleDead(_configuration.LocalEndPoint,
                new DeadMessage(
                    _configuration.Name,
                    GetNextIncarnationNumber()));

            bool anyAlive = false;
            lock(_lock)
            {
                anyAlive = _nodes.Count > 0;
            }

            if(anyAlive && !_leaveEvent.WaitOne(timeout))
            {
                _configuration.Logger.Warning("Leave message was not broadcast in the provided timeout of {Timeout}", timeout);
            }
        }

        private void Broadcast()
        {
            bool acquiredLock = false;
            try
            {
                acquiredLock = Monitor.TryEnter(_broadcastLock, TimeSpan.Zero);
                if (!acquiredLock) return;

                TrackedGossipNode node;
                lock (_lock)
                {
                    if (_nodes.Count == 1) return;

                    // TODO: keep track of last index so we can always make progress
                    var index = _random.Next(0, _nodes.Count - 1);
                    node = _nodes[index];
                }

                _messagePump.Send(node.IPEndPoint, new PingMessage(GetNextSequenceNumber()));
            }
            finally
            {
                if(acquiredLock)
                {
                    Monitor.Exit(_broadcastLock);
                }
            }
        }

        private int GetNextIncarnationNumber()
        {
            return Interlocked.Increment(ref _incarnationNumber);
        }

        private int GetNextSequenceNumber()
        {
            return Interlocked.Increment(ref _sequenceNumber);
        }

        private int GetRetransmitCount()
        {
            return (int)Math.Log(_nodes.Count);
        }

        private void HandleAck(IPEndPoint remoteEndPoint, AckMessage message)
        {
            // do nothing yet...
        }

        private void HandleAlive(IPEndPoint remoteEndPoint, AliveMessage message)
        {
            TrackedGossipNode node;
            GossipNodeState oldState;
            lock (_lock)
            {
                if (!_nodes.TryGetValue(message.Name, out node))
                {
                    node = new TrackedGossipNode(message.Name, message.IPEndPoint, message.Metadata)
                    {
                        Incarnation = int.MinValue,
                        State = GossipNodeState.Dead,
                        UpdatedAtUtc = DateTime.MinValue
                    };

                    if (_nodes.Count > 0)
                    {
                        int randomIndex = _random.Next(0, _nodes.Count - 1);
                        var temp = _nodes[randomIndex];
                        _nodes[randomIndex] = node;
                        _nodes.Add(temp);
                    }
                    else
                    {
                        _nodes.Add(node);
                    }
                }

                if (!node.IPEndPoint.Equals(message.IPEndPoint))
                {
                    _configuration.Logger.Error("Conflicting endpoints for {Name}. Mine: {MyEndPoint} Theirs: {TheirEndPoint}", node.Name, node.IPEndPoint, message.IPEndPoint);
                    return;
                }

                if (node.Incarnation >= message.Incarnation)
                {
                    _configuration.Logger.Verbose("Received old incarnation alive request for {Name}. Mine: {MyIncarnation} Theirs: {TheirIncarnation}", node.Name, node.Incarnation, message.Incarnation);
                    return;
                }

                oldState = node.State;
                node.State = GossipNodeState.Alive;
                node.Incarnation = message.Incarnation;
                node.UpdatedAtUtc = DateTime.UtcNow;
            }

            _messagePump.Broadcast(message, null);

            if (oldState == GossipNodeState.Dead)
            {
                if (NodeJoined != null)
                {
                    NodeJoined(node);
                }
            }
        }

        private void HandleDead(IPEndPoint remoteEndPoint, DeadMessage message)
        {
            TrackedGossipNode node;
            lock(_lock)
            {
                if (!_nodes.TryGetValue(message.Name, out node)) return;
                if (message.Incarnation < node.Incarnation) return;
                if (node.State == GossipNodeState.Dead) return;

                if (node.Name == _configuration.Name)
                {
                    // we need to rebut this if we aren't leaving
                    if (_leaveEvent == null)
                    {
                        int incarnation = GetNextIncarnationNumber();
                        while ((incarnation = GetNextIncarnationNumber()) <= message.Incarnation) ;

                        var alive = new AliveMessage(
                            _configuration.Name,
                            _configuration.LocalEndPoint,
                            _configuration.Metadata,
                            incarnation);
                        _messagePump.Broadcast(alive, null);
                        node.Incarnation = incarnation;
                        return;
                    }
                }

                node.Incarnation = message.Incarnation;
                node.State = GossipNodeState.Dead;
                node.UpdatedAtUtc = DateTime.UtcNow;

                _nodes.Remove(node);
            }

            // leave event will not be null if we are leaving.
            // therefore, we will wait in the shutdown method 
            // until this broadcast goes out.
            _messagePump.Broadcast(message, _leaveEvent);

            if(NodeLeft != null)
            {
                NodeLeft(node);
            }
        }

        private void HandlePing(IPEndPoint remoteEndPoint, PingMessage message)
        {
            var ack = new AckMessage(message.SequenceNumber);
            _messagePump.Send(remoteEndPoint, ack);
        }

        private void OnMessageReceived(IPEndPoint remoteEndPoint, GossipMessage message)
        {
            _configuration.Logger.Verbose("Received {@Message} from {RemoteEndPoint}", message, remoteEndPoint);
            switch (message.Type)
            {
                case GossipMessageType.Ping:
                    HandlePing(remoteEndPoint, (PingMessage)message);
                    break;
                case GossipMessageType.Ack:
                    HandleAck(remoteEndPoint, (AckMessage)message);
                    break;
                case GossipMessageType.Alive:
                    HandleAlive(remoteEndPoint, (AliveMessage)message);
                    break;
                case GossipMessageType.Dead:
                    HandleDead(remoteEndPoint, (DeadMessage)message);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void SetAlive()
        {
            HandleAlive(
                _configuration.LocalEndPoint,
                new AliveMessage(_configuration.Name,
                    _configuration.LocalEndPoint,
                    _configuration.Metadata,
                    GetNextIncarnationNumber()));
        }

        private void ThrowIfDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private class TrackedGossipNode : GossipNode
        {
            public TrackedGossipNode(string name, IPEndPoint ipEndPoint, byte[] metadata)
                : base(name, ipEndPoint, metadata)
            { }

            public int Incarnation { get; set; }

            public GossipNodeState State { get; set; }

            public DateTime UpdatedAtUtc { get; set; }
        }
    }
}