using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GossipNet
{
    public class GossipNode : IDisposable
    {
        private readonly GossipNodeConfiguration _configuration;
        private readonly GossipUdpListener _udpListener;

        public GossipNode(GossipNodeConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            _configuration = configuration;
            _udpListener = new GossipUdpListener(configuration.LocalEndPoint, HandleUdpPacket);

            _udpListener.Open();
        }

        public void Join(IPEndPoint remoteEndPoint)
        {
            var client = new UdpClient();
            var bytes = Encoding.ASCII.GetBytes("Hello");
            client.Send(bytes, bytes.Length, remoteEndPoint);
        }

        public void Dispose()
        {
            _udpListener.Close();
        }

        private void HandleUdpPacket(IPEndPoint remoteEndPoint, byte[] buffer)
        {
            var s = Encoding.ASCII.GetString(buffer);
            Console.WriteLine("Received from {0}: {1}", remoteEndPoint, s);
        }
    }
}