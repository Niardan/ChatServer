using System.Windows.Forms;
using Network.Network;
using Network.Owner;

namespace ChatClient.Callbacks
{
  
    public class AuthorizedCallbacks : Network.Callbacks.Callbacks
    {
        private readonly ProtocolUdpNetwork _network;
        private readonly IOwner _owner;

        private readonly ChatClient _chatClient;

        public AuthorizedCallbacks(ProtocolUdpNetwork network, IOwner owner, ChatClient chatClient)
        {
            _network = network;
            _owner = owner;
            _chatClient = chatClient;
        }

        public override void Ack(string value)
        {
            _network.Authorize(_owner, true);
           _chatClient.SuccessAuthorize();
        }

        public override void Fail(string value)
        {
            _network.Authorize(_owner, false);
          _chatClient.FailAuthorize(value);
        }
    }
}