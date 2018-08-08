using ChatServer.Messages;
using ChatServer.Transport;
using ChatServer.Values;
using MessagePack;

namespace ChatServer.Protocol
{
    public class TransportUdpProtocol : UdpProtocol
    {
        private readonly IUdpTransport _transport;
        private readonly int _maxMessageSize;
        
        private const string _ack = "ack";
        private const string _fail = "fail";
        
        private const string _tooBigMessage = "toBigMessage";
        private const string _invalidMessage = "invalidMessage";
        private const string _invalidAuthorize = "invalidAuthorize";
        private const string _invalidRequest = "invalidRequest";
        private const string _invalidResponse = "invalidResponse";

        public TransportUdpProtocol(IUdpTransport transport, int maxMessageSize)
        {
            _transport = transport;
            _maxMessageSize = maxMessageSize;
        }

        public override bool Start(int port)
        {
            if (_transport.Start(port))
            {
                _transport.Connected += OnTransportConnected;
                _transport.Disconnected += OnTransportDisconnected;
                _transport.Received += OnTransportReceived;

                return true;
            }

            return false;
        }

        public override void Stop()
        {
            _transport.Connected -= OnTransportConnected;
            _transport.Disconnected -= OnTransportDisconnected;
            _transport.Received -= OnTransportReceived;

            _transport.Stop();
        }

        public override void Update()
        {
            _transport.Update();
        }

        public override void Connect(string host, int port)
        {
            _transport.Connect(host, port);
        }

        public override void Disconnect(string address)
        {
            _transport.Disconnect(address);
        }

        public override void Request(string address, long id, IValue1 value)
        {
            Send(address, new RequestMessage(id, value));
        }

        public override void Ack(string address, long id, string text)
        {
            Response(address, _ack, id, text);
        }

        public override void Fail(string address, long id, string text)
        {
            Response(address, _fail, id, text);
        }

        public override void Error(string address, string text)
        {
            var message = new ErrorMessage(-1, text);
            Send(address, message);
        }

        private void Response(string address, string response, int id, string text)
        {
            var message = new ResponseMessage(id, new ResponseValue(response, text));
            Send(address, message);
        }

        private void Send(string address, IMessage message)
        {
            _transport.Send(address, MessagePackSerializer.Serialize(message));
        }

        private void OnTransportConnected(IUdpTransport transport, string address)
        {
            CallConnected(address);
        }

        private void OnTransportDisconnected(IUdpTransport transport, string address)
        {
            CallDisconnected(address);
        }

        private void OnTransportReceived(IUdpTransport transport, string address, byte[] bytes)
        {
            if (bytes.Length <= _maxMessageSize)
            {
                var message = MessagePackSerializer.Deserialize<IMessage>(bytes);

                switch (message.TypeMessage)
                {
                    case "authorize":
                        ReceiveAuthorize(address, message);
                        break;
                    case "request":
                        ReceiveRequest(address, message);
                        break;
                    case "response":
                        ReceiveResponse(address, message);
                        break;
                    default:
                        Error(address, _invalidMessage);
                        break;
                }
            }
            else
            {
                Error(address, _tooBigMessage);
            }
        }

        private void ReceiveAuthorize(string address, IMessage message)
        {
            var autorizeMessage = message as AuthorizeMessage;
            if (autorizeMessage != null && autorizeMessage.Name != null)
            {
                CallAuthorizeReceived(address, autorizeMessage.Name);
            }
            else
            {
                Error(address, _invalidAuthorize);
            }
        }

        private void ReceiveRequest(string address, IMessage message)
        {
            var receiveMessage = message as RequestMessage;

            if (receiveMessage != null && receiveMessage.Id > -1)
            {
                CallRequestReceived(address, receiveMessage.Id, receiveMessage.Value);
            }
            else
            {
                Error(address, _invalidRequest);
            }
        }

        private void ReceiveResponse(string address, IMessage message)
        {
            var receiveMessage = message as ResponseMessage;

            if (receiveMessage != null && receiveMessage.Id > -1)
            {
                CallResponseReceived(address, receiveMessage.Id, receiveMessage.Answer);
            }
            else
            {
                Error(address, _invalidResponse);
            }
        }
    }
}
