using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet
{
    internal class GossipUdpListener
    {
        private readonly UdpClient _client;
        private readonly Action<IPEndPoint, byte[]> _onReceive;
        private bool _closed;

        public GossipUdpListener(IPEndPoint localEndPoint, Action<IPEndPoint, byte[]> onReceive)
        {
            Debug.Assert(localEndPoint != null);
            Debug.Assert(onReceive != null);

            _client = new UdpClient(localEndPoint);
            _onReceive = onReceive;
        }

        public void Close()
        {
            _closed = true;
            _client.Close();
        }

        public void Open()
        {
            _client.BeginReceive(Receive, null);
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

            if (!_closed)
            {
                _client.BeginReceive(Receive, null);
                _onReceive(remoteEndPoint, data);
            }
        }
    }
}
