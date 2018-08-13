using ChatClient.Callbacks;
using Network.Network;
using Network.Owner;
using Network.Transport;
using Network.Values;

namespace ChatClient
{
    public delegate void MessageHandler(string text, bool system);
    public delegate void ChangeStageHandler(ClientStage stage);

    public class ChatClient
    {
        private readonly ProtocolUdpNetwork _network;
        private readonly IUdpTransport _transport;
        private IOwner _owner;
        private string _name;
        private int _maxMessageLenght;

        public ChatClient(IUdpTransport transport, ProtocolUdpNetwork network, int maxMessageLenght)
        {
            _transport = transport;
            _transport.Disconnected += TransportOnDisconnected;

            _network = network;
            _maxMessageLenght = maxMessageLenght;
            _network.RequestReceived += NetworkOnRequestReceived;
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
            if (string.IsNullOrEmpty(text))
            {
                CallMessage("Fail. Message empty.", true);
            }

            else if (text.Length > _maxMessageLenght)
            {
                CallMessage("Fail. Message to big.", true);
            }

            else
            {
                var value = new ChatValue(_name, text);
                _network.Request(_owner, value, new ChatMessageCallbacks(this));
                string message = _name + ": " + text;
                CallMessage(message, false);
            }
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
            CallMessage("Server chat send fail. Reason: " + reason, true);
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

        private void TransportOnDisconnected(IUdpTransport transport, string address)
        {
            CallChangeStage(ClientStage.Disconnected);
            CallMessage("Disconnect", true);
        }

        private void NetworkOnRequestReceived(IUdpNetwork network, IOwner owner, IValue request, Network.Callbacks.ICallbacks callbacks)
        {
            callbacks.Ack("ok");
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