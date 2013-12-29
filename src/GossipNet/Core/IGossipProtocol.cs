using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GossipNet.Core
{
    public interface IGossipProtocol<TMessage> : IDisposable
    {
        void Disseminate(TMessage message);
    }
}