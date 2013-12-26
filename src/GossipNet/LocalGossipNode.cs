using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GossipNet.IO;
using GossipNet.Messages;

namespace GossipNet
{
    public class LocalGossipNode : IDisposable
    {
        private readonly GossipNodeConfiguration _configuration;
        private readonly GossipMessagePump _messagePump;
        private readonly GossipNodeCollection _nodes;

        private int _incarnationNumber;
        private int _sequenceNumber;

        public LocalGossipNode(GossipNodeConfiguration configuration)
        {
            _configuration = configuration;

            _nodes = new GossipNodeCollection(_configuration.Logger);

            _configuration.Logger.Information("Started at {LocalEndPoint}", configuration.LocalEndPoint);

            var codec = new GossipMessageCodec();
            _messagePump = new GossipMessagePump(configuration.LocalEndPoint, 
                codec, 
                codec, 
                configuration.Logger);

            _messagePump.MessageReceived += OnMessageReceived;

            _messagePump.Open(() => 3);
            SetAlive();
        }

        public event Action<GossipNode> NodeJoined
        {
            add { _nodes.NodeJoined += value; }
            remove { _nodes.NodeJoined -= value; }
        }

        public event Action<GossipNode> NodeLeft
        {
            add { _nodes.NodeLeft += value; }
            remove { _nodes.NodeLeft -= value; }
        }

        public void Dispose()
        {
            _messagePump.MessageReceived -= OnMessageReceived;
            _messagePump.Dispose();
        }

        public IReadOnlyList<GossipNode> GetAllNodes()
        {
            return _nodes.AsReadOnly();
        }

        public void JoinCluster(IPEndPoint primary, params IPEndPoint[] other)
        {
            // TODO: this should be a full state transfer, but for now it
            // is just a ping that will likely piggyback an alive message, and
            // each node will then have 2 nodes in their list.
            _messagePump.Send(primary, new PingMessage(GetNextSequenceNumber()));
        }

        public void LeaveCluster()
        {
            throw new NotImplementedException();
        }

        private int GetNextIncarnationNumber()
        {
            return Interlocked.Increment(ref _incarnationNumber);
        }

        private int GetNextSequenceNumber()
        {
            return Interlocked.Increment(ref _sequenceNumber);
        }

        private void HandleAck(IPEndPoint remoteEndPoint, AckMessage message)
        {
            // do nothing yet...
        }

        private void HandleAlive(IPEndPoint remoteEndPoint, AliveMessage message)
        {
            _nodes.MarkNodeAsAlive(message);
            _messagePump.Broadcast(message);
        }

        private void HandlePing(IPEndPoint remoteEndPoint, PingMessage message)
        {
            var ack = new AckMessage(message.SequenceNumber);
            _messagePump.Send(remoteEndPoint, ack);
        }

        private void OnMessageReceived(IPEndPoint remoteEndPoint, GossipMessage message)
        {
            _configuration.Logger.Verbose("Received {@Message} from {RemoteEndPoint}", message, remoteEndPoint);
            switch (message.MessageType)
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
                default:
                    throw new NotSupportedException();
            }
        }

        private void SetAlive()
        {
            // includes the local node in the node list as an alive node.
            // in addition, it will then include this message in the broadcast
            // queue for dissemination.
            HandleAlive(
                _configuration.LocalEndPoint,
                new AliveMessage(_configuration.Name,
                    _configuration.LocalEndPoint,
                    _configuration.Metadata,
                    GetNextIncarnationNumber()));
        }
    }
}