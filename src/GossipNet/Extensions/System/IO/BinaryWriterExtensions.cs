using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    internal static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, IPEndPoint endPoint)
        {
            var addressBytes = endPoint.Address.GetAddressBytes();
            writer.Write((byte)addressBytes.Length);
            writer.Write(endPoint.Port);
        }
    }
}