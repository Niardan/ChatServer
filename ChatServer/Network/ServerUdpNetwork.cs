using System.Collections.Generic;
using ChatServer.Callbacks;
using ChatServer.Owner;
using ChatServer.Protocol;
using ChatServer.Response;
using ChatServer.Values;

namespace ChatServer.Network
{
    public abstract class ServerUdpNetwork : UdpNetwork
    {
        private readonly IUdpProtocol _protocol;
        private readonly int _timeout;

        private readonly ICallbacks _noCallbacks = new NoCallbacks();
        private readonly IResponse _downMessage = new FailResponse("down");
        private readonly IResponse _disconnectedMessage = new FailResponse("disconnected");
        private readonly IResponse _timeoutMessage = new FailResponse("timeout");

        private readonly string _alreadyAuthorized = "alreadyAuthorized";
      
        private readonly string _notAuthorizedRequest = "notAuthorizedRequest";
        private readonly string _notAuthorizedResponse = "notAuthorizedResponse";

        class ServerPeer
        {
            private readonly string _address;
            private readonly string _destination;
            private readonly string _peer;
            private readonly IUdpProtocol _protocol;
            private readonly LinkedList<RequestInfo> _sent;

            private readonly IDictionary<long, LinkedListNode<RequestInfo>> _requests = new Dictionary<long, LinkedListNode<RequestInfo>>();
            private int _count;

            public ServerPeer(string address, string destination, string peer, IUdpProtocol protocol, LinkedList<RequestInfo> sent)
            {
                _address = address;
                _destination = destination;
                _peer = peer;
                _protocol = protocol;
                _sent = sent;
            }

            public string Address { get { return _address; } }
            public string Destination { get { return _destination; } }
            public string Peer { get { return _peer; } }

            public void Request(IOwner owner, IValue1 value)
            {
                var node = _sent.AddLast(new RequestInfo((long)_now.Get, owner, _count, this));
                _requests.Add(_count, node);
                _protocol.Request(_address, _count, value);
                ++_count;
            }

            public void RemoveRequest(long id)
            {
                _requests.Remove(id);
            }

            public bool TryPopRequest(long id, out IOwner owner)
            {
                LinkedListNode<RequestInfo> node;
                if (_requests.TryGetValue(id, out node))
                {
                    _sent.Remove(node);
                    _requests.Remove(id);
                    owner = node.Value.Owner;
                    return true;
                }

                owner = null;
                return false;
            }

            public IEnumerable<LinkedListNode<RequestInfo>> Requests { get { return _requests.Values; } }
        }

        private readonly IDictionary<string, ServerPeer> _fromAddress = new Dictionary<string, ServerPeer>();
        private readonly IDictionary<string, ServerPeer> _fromPeer = new Dictionary<string, ServerPeer>();

        private readonly ICollection<string> _authorizing = new HashSet<string>();

        class RequestInfo
        {
            private readonly long _time;
            private readonly IOwner _owner;
            private readonly long _id;
            private readonly ServerPeer _peer;

            public RequestInfo(long time, IOwner owner, long id, ServerPeer peer)
            {
                _time = time;
                _owner = owner;
                _id = id;
                _peer = peer;
            }

            public long Time { get { return _time; } }
            public IOwner Owner { get { return _owner; } }
            public long Id { get { return _id; } }
            public ServerPeer Peer { get { return _peer; } }
        }

        private readonly LinkedList<RequestInfo> _sent = new LinkedList<RequestInfo>();

        public ServerUdpNetwork(IUdpProtocol protocol, int timeout)
        {
            _protocol = protocol;
            _timeout = timeout;
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

        public override void Request(IOwner owner, string destination, string peer, IValue value)
        {
            ServerPeer serverPeer;
            if (_fromPeer.TryGetValue(peer, out serverPeer))
            {
                serverPeer.Request(owner, value);
            }
            else
            {
                CallResponseReceived(owner.Id, destination, _downMessage);
            }
        }

        private void OnProtocolConnected(IUdpProtocol protocol, string address)
        {
        }

        private void OnProtocolDisconnected(IUdpProtocol protocol, string address)
        {
            _authorizing.Remove(address);

            ServerPeer serverPeer;
            if (_fromAddress.TryGetValue(address, out serverPeer))
            {
                string peer = serverPeer.Peer;
                _fromPeer.Remove(peer);
                _fromAddress.Remove(address);

                foreach (var node in serverPeer.Requests)
                {
                    RequestInfo requestInfo = node.Value;
                    _sent.Remove(node);
                    CallResponseReceived(requestInfo.Id, serverPeer.Destination, _disconnectedMessage);
                }

                CallDisconnected(serverPeer.Destination, peer, _noCallbacks);
            }
        }

        private void OnProtocolAuthorizeReceived(IUdpProtocol protocol, string address, string destination, string peer, string token)
        {
            if (!_fromAddress.ContainsKey(address))
            {
                if (!_authorizing.Contains(address))
                {
                    _authorizing.Add(address);
                    CallAuthorizeReceived(destination, peer, token, new UdpNetworkConnectCallbacks(this, address, destination, peer));
                }
                else
                {
                    _protocol.Error(address, _alreadyAuthorizing);
                }
            }
            else
            {
                _protocol.Error(address, _alreadyAuthorized);
            }
        }

        private void OnProtocolRequestReceived(IUdpProtocol protocol, string address, long id, IValue value)
        {
            ServerPeer serverPeer;
            if (_fromAddress.TryGetValue(address, out serverPeer))
            {
                string peer = serverPeer.Peer;
                CallRequestReceived(serverPeer.Destination, peer, value, new UdpNetworkRequestCallback(this, peer, id));
            }
            else
            {
                _protocol.Error(address, _notAuthorizedRequest);
            }
        }

        private void OnProtocolResponseReceived(IUdpProtocol protocol, string address, long id, IValue value)
        {
            ServerPeer serverPeer;
            if (_fromAddress.TryGetValue(address, out serverPeer))
            {
                IOwner owner;
                if (serverPeer.TryPopRequest(id, out owner))
                {
                    CallResponseReceived(owner.Id, owner.Recipient, new AckResponse(value));
                }
            }
            else
            {
                _protocol.Error(address, _notAuthorizedResponse);
            }
        }

        public void Authorize(string address, string destination, string peer)
        {
            _authorizing.Remove(address);

            ServerPeer serverPeer = new ServerPeer(address, destination, peer, _protocol, _now, _sent);
            _fromAddress.Add(address, serverPeer);
            _fromPeer.Add(peer, serverPeer);

            _protocol.Authorize(address);
        }

        public void AuthorizeError(string address, IValue value)
        {
            _authorizing.Remove(address);
            _protocol.Error(address, value);
        }

        public void Ack(string peer, long id, string value)
        {
            ServerPeer serverPeer;
            if (_fromPeer.TryGetValue(peer, out serverPeer))
            {
                _protocol.Ack(serverPeer.Address, id, value);
            }
        }

        public void Fail(string peer, long id, IValue value)
        {
            ServerPeer serverPeer;
            if (_fromPeer.TryGetValue(peer, out serverPeer))
            {
                _protocol.Fail(serverPeer.Address, id, value);
            }
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
                requestInfo.Peer.RemoveRequest(requestInfo.Id);
                IOwner owner = requestInfo.Owner;
                CallResponseReceived(owner.Id, owner.Recipient, _timeoutMessage);
            }
        }

        public override void Disconnect(string peer)
        {
            ServerPeer serverPeer;
            if (_fromPeer.TryGetValue(peer, out serverPeer))
            {
                _protocol.Disconnect(serverPeer.Address);
            }
        }
    }
}