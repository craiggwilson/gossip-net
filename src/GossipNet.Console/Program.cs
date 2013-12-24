using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new[] { "20000" };
            }

            var port = int.Parse(args[0]);
            int? joinPort = null;
            if(args.Length == 2)
            {
                joinPort = int.Parse(args[1]);
            }

            var config = GossipNodeConfiguration.Create(x =>
            {
                x.LocalEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            });

            var node = new GossipNode(config);

            if(joinPort != null)
            {
                node.Join(new IPEndPoint(IPAddress.Loopback, joinPort.Value));
            }

            System.Console.ReadKey();
            node.Dispose();
        }
    }
}