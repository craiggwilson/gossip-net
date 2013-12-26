using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GossipNet.IO;
using GossipNet.Messages;
using GossipNet.Support;
using Serilog;

namespace GossipNet
{
    internal class GossipNodeCollection
    {
        private readonly object _lock = new object();
        private readonly ILogger _logger;
        private readonly ListMap<string, TrackedGossipNode> _nodes;
        private readonly Random _random;

        public GossipNodeCollection(ILogger logger)
        {
            Debug.Assert(logger != null);

            _logger = logger;
            _nodes = new ListMap<string, TrackedGossipNode>(n => n.Name);
            _random = new Random();
        }

        public event Action<GossipNode> NodeJoined;
        public event Action<GossipNode> NodeLeft;

        public void MarkNodeAsAlive(AliveMessage message)
        {
            lock (_lock)
            {
                TrackedGossipNode node;
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
                    _logger.Error("Conflicting endpoints for {Name}. Mine: {MyEndPoint} Theirs: {TheirEndPoint}", node.Name, node.IPEndPoint, message.IPEndPoint);
                    return;
                }

                if (node.Incarnation >= message.Incarnation)
                {
                    _logger.Verbose("Received old incarnation alive request for {Name}. Mine: {MyIncarnation} Theirs: {TheirIncarnation}", node.Name, node.Incarnation, message.Incarnation);
                    return;
                }

                var oldState = node.State;
                node.State = GossipNodeState.Alive;
                node.Incarnation = message.Incarnation;
                node.UpdatedAtUtc = DateTime.UtcNow;

                if(oldState == GossipNodeState.Dead)
                {
                    if(NodeJoined != null)
                    {
                        NodeJoined(node);
                    }
                }
            }
        }

        public IReadOnlyList<GossipNode> AsReadOnly()
        {
            return _nodes.ToList().AsReadOnly();
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