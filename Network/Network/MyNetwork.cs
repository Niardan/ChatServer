using System.Collections.Generic;
using Network.Callbacks;
using Network.Owner;
using Network.Protocol;
using Network.Utils;
using Network.Values;

namespace Network.Network
{
    public class MyNetwork : UdpNetwork
    {
        private readonly INow _now;
        private readonly IUdpProtocol _protocol;
        private readonly IDictionary<string, IOwner> _toOwner = new Dictionary<string, IOwner>();
        private readonly ICollection<string> _toAutorized = new HashSet<string>();
        private readonly ICollection<string> _authorizing = new HashSet<string>();

        private const long _timeout = 5000;

        private readonly IDictionary<long, ICallbacks> _callbacks = new Dictionary<long, ICallbacks>();
        private long _countRequest = 0;

        private readonly string _alreadyAuthorized = "alreadyAuthorized";
        private readonly string _notAuthorized = "notAuthorized";
        private readonly string _timeoutMessage = "timeout";
        private readonly string _alreadyResponse = "alreadyResponse";
        private readonly LinkedList<RequestInfo> _sent = new LinkedList<RequestInfo>();

        public MyNetwork(IUdpProtocol protocol)
        {
            _protocol = protocol;
        }

        public override bool Start(int port)
        {
            if (_protocol.Start(port))
            {
                _protocol.Connected += OnProtocolConnected;
                _protocol.Disconnectd += OnProtocolDisconnected;
                _protocol.AuthorizeReceived += OnProtocolAuthorizeReceived;
                _protocol.RequestReceived += OnProtocolRequestReceived;
                _protocol.ResponseReceived += OnProtocolResponseReceived;

                return true;
            }

            return false;
        }

        public override void Stop()
        {
            _protocol.Connected -= OnProtocolConnected;
            _protocol.Disconnectd -= OnProtocolDisconnected;
            _protocol.AuthorizeReceived -= OnProtocolAuthorizeReceived;
            _protocol.RequestReceived -= OnProtocolRequestReceived;
            _protocol.ResponseReceived -= OnProtocolResponseReceived;

            _protocol.Stop();
        }

        private void OnProtocolConnected(IUdpProtocol protocol, string address)
        {
            IOwner owner = new Owner.Owner(address);
            _toOwner.Add(address, owner);
        }

        private void OnProtocolDisconnected(IUdpProtocol protocol, string address)
        {
            IOwner owner = _toOwner[address];
            CallDisconnected(owner);
            _toOwner.Remove(address);
        }

        private void OnProtocolAuthorizeReceived(IUdpProtocol protocol, string address, long id, string name)
        {
            if (!_authorizing.Contains(address) || !_toAutorized.Contains(address))
            {
                IOwner owner = _toOwner[address];
                CallAuthorizeReceived(owner, name, new UdpNetworkRequestCallback(protocol, address, id));
            }
            else
            {
                protocol.Error(address, _alreadyAuthorized);
            }
        }

        public void Authorize(IOwner owner, bool success)
        {
            _authorizing.Remove(owner.Id);
            if (success)
            {
                _toAutorized.Add(owner.Id);
            }
            else
            {
                _protocol.Disconnect(owner.Id);
            }
        }

        private void OnProtocolResponseReceived(IUdpProtocol protocol, string address, long id, IValue message)
        {
            var response = message as ResponseValue;
            if (response != null)
            {
                if (response.Answer == "ack")
                {
                    _callbacks[id].Ack(response.Text);
                }
                else
                {
                    _callbacks[id].Fail(response.Text);
                }
            }
            else
            {
                _protocol.Error(address, _notAuthorized);
            }
        }

        private void OnProtocolRequestReceived(IUdpProtocol protocol, string address, long id, IValue message)
        {
            if (_toAutorized.Contains(address))
            {
                IOwner owner = _toOwner[address];
                CallRequestReceived(owner, message, new UdpNetworkRequestCallback(protocol, address, id));
            }
            else
            {
                protocol.Error(address, _alreadyAuthorized);
            }
        }

        public override void Update()
        {

            _protocol.Update();

            while (_sent.Count > 0)
            {
                var node = _sent.First;
                var requestInfo = node.Value;

                long ts = (long)_now.Get;
                if (ts < requestInfo.Time + _timeout)
                {
                    break;
                }

                _sent.Remove(node);
                var callback = _callbacks[requestInfo.Id];
                _callbacks.Remove(requestInfo.Id);
                IOwner owner = requestInfo.Owner;
                callback.Fail(_timeoutMessage);
            }
        }

        public override void Request(IOwner owner, IValue value, ICallbacks callbacks)
        {
            if (_toAutorized.Contains(owner.Id))
            {
                long id = _countRequest;
                _countRequest++;
                _sent.AddLast(new RequestInfo(owner, id, _now.Get));
                _callbacks.Add(id, callbacks);
                _protocol.Request(owner.Id, id, value);
            }
        }

        public override void Disconnect(IOwner owner)
        {
            string address = owner.Id;
            _toOwner.Remove(address);
            _toAutorized.Remove(address);
            _protocol.Disconnect(address);
        }
    }

    public class RequestInfo
    {
        private readonly IOwner _owner;
        private readonly long _id;
        private readonly double _time;

        public RequestInfo(IOwner owner, long id, double time)
        {
            _owner = owner;
            _id = id;
            _time = time;
        }

        public IOwner Owner
        {
            get { return _owner; }
        }

        public long Id
        {
            get { return _id; }
        }

        public double Time
        {
            get { return _time; }
        }
    }
}