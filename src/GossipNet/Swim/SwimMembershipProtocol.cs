using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GossipNet.Support;
using GossipNet.Swim.Messages;

namespace GossipNet.Swim
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMember"></typeparam>
    /// <remarks>http://www.cs.cornell.edu/~asdas/research/dsn02-swim.pdf</remarks>
    public class SwimMembershipProtocol
    {
        private readonly SwimConfiguration _config;
        private readonly DelegateKeyedCollection<string, SwimMember> _members;
        private readonly ISwimMessageService _messageService;

        public SwimMembershipProtocol(SwimConfiguration config)
        {
            Debug.Assert(config != null);

            _config = config;

            _members = new DelegateKeyedCollection<string, SwimMember>(x => x.Id);
            var messageCodec = new SwimMessageCodec();
            _messageService = new UdpSwimMessageService(
                config, 
                messageCodec, 
                () => _config.RetransmitCountCalculator(_members.Count));

            _messageService.MessageReceived += OnMessageReceived;
        }

        public event Action<SwimMember> MemberJoined;

        public event Action<SwimMember> MemberLeft;

        public IReadOnlyList<SwimMember> Members
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {

        }

        public void Disseminate(SwimMessage message)
        {
            throw new NotImplementedException();
        }

        public void JoinCluster()
        {
            throw new NotImplementedException();
        }

        public void LeaveCluster(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        private void OnMessageReceived(ReceivedSwimMessage receivedMessage)
        {
            switch(receivedMessage.Message.Type)
            {
                case SwimMessageType.Ping:
                    HandlePing((PingMessage)receivedMessage.Message, receivedMessage.RemoteEndPoint);
                    return;
            }
        }

        private void HandlePing(PingMessage message, IPEndPoint remoteEndPoint)
        {
            
        }
    }
}