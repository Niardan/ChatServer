using System.Windows.Forms;
using Network.Network;
using Network.Owner;

namespace ChatClient.Callbacks
{
    public delegate void AuthorizedSuccessHandler();
    public delegate void AuthorizedFailHandler(string reason);
    public class AuthorizedCallbacks : Network.Callbacks.Callbacks
    {
        private readonly MyNetwork _network;
        private readonly IOwner _owner;
        private readonly AuthorizedSuccessHandler _authorizedSuccess;
        private readonly AuthorizedFailHandler _authorizedFail;
        public AuthorizedCallbacks(MyNetwork network, IOwner owner, AuthorizedSuccessHandler authorizedSuccess, AuthorizedFailHandler authorizedFail)
        {
            _network = network;
            _owner = owner;
            _authorizedSuccess = authorizedSuccess;
            _authorizedFail = authorizedFail;
        }

        public override void Ack(string value)
        {
            _network.Authorize(_owner, true);
           _authorizedSuccess.Invoke();
        }

        public override void Fail(string value)
        {
            _network.Authorize(_owner, false);
           _authorizedFail.Invoke(value);
        }
    }
}