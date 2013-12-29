using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    internal static class BinaryReaderExtensions
    {
        public static IPEndPoint ReadIPEndPoint(this BinaryReader reader)
        {
            var length = reader.ReadByte();
            var addressBytes = reader.ReadBytes(length);
            var port = reader.ReadInt32();
            return new IPEndPoint(new IPAddress(addressBytes), port);
        }
    }
}