using Network.Messages;
using Network.Transport;
using Network.Utils;
using Network.Values;

namespace Network.Protocol
{
    public class TransportUdpProtocol : UdpProtocol
    {
        private readonly IUdpTransport _transport;
        private readonly ISerializer _serializer;
        private readonly int _maxMessageSize;

        private readonly string _ack = "ack";
        private readonly string _fail = "fail";

        private readonly string _tooBigMessage = "toBigMessage";
        private readonly string _invalidMessage = "invalidMessage";
        private readonly string _invalidAuthorize = "invalidAuthorize";
        private readonly string _invalidRequest = "invalidRequest";
        private readonly string _invalidResponse = "invalidResponse";

        public TransportUdpProtocol(IUdpTransport transport, int maxMessageSize, ISerializer serializer)
        {
            _transport = transport;
            _maxMessageSize = maxMessageSize;
            _serializer = serializer;
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

        public override void Request(string address, int id, IValue value)
        {
            Send(address, new RequestMessage(id, value));
        }

        public override void Authorize(string address, int id, string name)
        {
            Send(address, new AuthorizeMessage(id, name));
        }

        public override void Ack(string address, int id, string text)
        {
            Response(address, _ack, id, text);
        }

        public override void Fail(string address, int id, string text)
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
            _transport.Send(address, _serializer.Serialize(message));
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
                var message = _serializer.Deserialize<IMessage>(bytes);

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
                    case "error":
                        ReceiveError(address, message);
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

        private void ReceiveError(string address, IMessage message)
        {
            var errorMessage = message as ErrorMessage;
            if (errorMessage != null)
            {
                CallErrorReceived(address, errorMessage.Error);
            }
        }

        private void ReceiveAuthorize(string address, IMessage message)
        {
            var autorizeMessage = message as AuthorizeMessage;
            if (autorizeMessage != null && autorizeMessage.Name != null)
            {
                CallAuthorizeReceived(address, autorizeMessage.Id, autorizeMessage.Name);
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
                CallRequestReceived(address, receiveMessage.Id, receiveMessage.MessageValue);
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
