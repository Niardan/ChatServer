using ChatClient.Callbacks;
using Network.Network;
using Network.Owner;
using Network.Protocol;
using Network.Transport;
using Network.Utils;
using Network.Values;

namespace ChatClient
{
    public delegate void MessageHandler(string text, bool system);
    public delegate void ChangeStageHandler(ClientStage stage);

    public class ChatClient
    {
        private readonly MyNetwork _network;
        private readonly IUdpTransport _transport;
        private IOwner _owner;
        private bool _isConnect;
        private string _name;

        public ChatClient(IUdpTransport transport,  MyNetwork network)
        {
            _transport = transport;
            _network = network;
            _network.RequestReceived += NetworkOnRequestReceived;
            _network.Disconnected += NetworkOnDisconnected;
            _network.Connected += NetworkOnConnected;
            _network.Start(0);
        }

        public event MessageHandler Message;
        public event ChangeStageHandler ChangeStage;

        private void CallMessage(string text, bool system)
        {
            if (Message != null)
            {
                Message(text, system);
            }
        }

        private void CallChangeStage(ClientStage stage)
        {
            if (ChangeStage != null)
            {
                ChangeStage(stage);
            }
        }

        public void Connect(string host, int port)
        {
            _transport.Connect(host, port);
            CallChangeStage(ClientStage.Connecting);
        }

        public void Disconnect()
        {
            _network.Disconnect(_owner);
            CallChangeStage(ClientStage.Disconnecting);
        }

        public void Authorize(string name)
        {
            _name = name;
            _network.Authorize(_owner, name, new AuthorizedCallbacks(_network, _owner, this));
            CallChangeStage(ClientStage.Authorizing);
        }

        public void SendMessage(string text)
        {
            var value = new ChatValue(_name, text);
            _network.Request(_owner, value, new ChatMessageCallbacks(this));
            string message = _name + ": " + text;
            CallMessage(message, false);
        }

        public void SuccessAuthorize()
        {
            CallChangeStage(ClientStage.Autorized);
            CallMessage("Authorized", true);
        }

        public void FailAuthorize(string reason)
        {
            CallChangeStage(ClientStage.Autorized);
            CallMessage("Authorized Fail, Reason: " + reason, true);
        }

        public void FailChat(string reason)
        {
            CallMessage("Chat Send Fail, Reason: " + reason, true);
        }

        public void Update()
        {
            _network.Update();
        }

        private void NetworkOnConnected(IUdpNetwork network, IOwner owner)
        {
            _owner = owner;
            CallChangeStage(ClientStage.Connected);
            CallMessage("Connected", true);
        }

        private void NetworkOnDisconnected(IUdpNetwork network, IOwner owner)
        {
            CallChangeStage(ClientStage.Disconnected);
            CallMessage("Disconnect", true);
        }

        private void NetworkOnRequestReceived(IUdpNetwork network, Network.Owner.IOwner owner, Network.Values.IValue request, Network.Callbacks.ICallbacks callbacks)
        {
            var value = (ChatValue)request;
            string message = value.Name + ": " + value.Message;
            CallMessage(message, false);
        }

    }

    public enum ClientStage
    {
        Disconnecting,
        Disconnected,
        Connecting,
        Connected,
        Authorizing,
        Autorized
    }
}