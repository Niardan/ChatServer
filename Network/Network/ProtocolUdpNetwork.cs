using System;
using System.Collections.Generic;
using Network.Callbacks;
using Network.Owner;
using Network.Protocol;
using Network.Utils;
using Network.Values;

namespace Network.Network
{
    public class ProtocolUdpNetwork : UdpNetwork
    {
        private readonly INow _now;
        private readonly IUdpProtocol _protocol;
        private readonly IDictionary<string, IOwner> _toOwner = new Dictionary<string, IOwner>();
        private readonly ICollection<string> _toAutorized = new HashSet<string>();
        private readonly ICollection<string> _authorizing = new HashSet<string>();

        private readonly int _timeout;

        private readonly IDictionary<long, Tuple<ICallbacks, RequestInfo>> _callbacks = new Dictionary<long, Tuple<ICallbacks, RequestInfo>>();
        private int _countRequest;

        private readonly string _alreadyAuthorized = "alreadyAuthorized";
        private readonly string _notAuthorized = "notAuthorized";
        private readonly string _timeoutMessage = "timeout";

        private readonly LinkedList<RequestInfo> _sent = new LinkedList<RequestInfo>();

        public ProtocolUdpNetwork(IUdpProtocol protocol, INow now, int timeout)
        {
            _protocol = protocol;
            _now = now;
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
            CallConnected(owner);
        }

        private void OnProtocolDisconnected(IUdpProtocol protocol, string address)
        {
            IOwner owner;
            if (_toOwner.TryGetValue(address, out owner))
            {
                _toOwner.Remove(address);
                _toAutorized.Remove(address);
                CallDisconnected(owner);
            }
        }

        private void OnProtocolAuthorizeReceived(IUdpProtocol protocol, string address, int id, string name)
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
        }

        private void OnProtocolResponseReceived(IUdpProtocol protocol, string address, int id, IValue message)
        {
            var response = message as ResponseValue;
            if (response != null)
            {
                var callback = _callbacks[id].Item1;
                _sent.Remove(_callbacks[id].Item2);
                if (response.Answer == "ack")
                {
                    callback.Ack(response.Text);
                }
                else
                {
                    callback.Fail(response.Text);
                }
            }
            else
            {
                _protocol.Error(address, _notAuthorized);
            }
        }

        private void OnProtocolRequestReceived(IUdpProtocol protocol, string address, int id, IValue message)
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
                var callback = _callbacks[requestInfo.Id].Item1;
                _callbacks.Remove(requestInfo.Id);
                callback.Fail(_timeoutMessage);
            }
        }

        public override void Request(IOwner owner, IValue value, ICallbacks callbacks)
        {
            if (_toAutorized.Contains(owner.Id))
            {
                int id = _countRequest;
                var requestInfo = new RequestInfo(owner, id, _now.Get);
                _sent.AddLast(requestInfo);
                _callbacks.Add(id, new Tuple<ICallbacks, RequestInfo>(callbacks, requestInfo));
                _protocol.Request(owner.Id, id, value);
                _countRequest++;
            }
        }

        public override void Authorize(IOwner owner, string name, ICallbacks callbacks)
        {
            string address = owner.Id;
            if (!_toAutorized.Contains(address))
            {
                int id = _countRequest;
                _countRequest++;
                var requestInfo = new RequestInfo(owner, id, _now.Get);
                _sent.AddLast(requestInfo);
                _callbacks.Add(id, new Tuple<ICallbacks, RequestInfo>(callbacks, requestInfo));
                _authorizing.Add(address);
                _protocol.Authorize(address, id, name);
            }
        }

        public override void Disconnect(IOwner owner)
        {
            string address = owner.Id;
            _protocol.Disconnect(address);
        }
    }

    public class RequestInfo
    {
        private readonly IOwner _owner;
        private readonly int _id;
        private readonly double _time;

        public RequestInfo(IOwner owner, int id, double time)
        {
            _owner = owner;
            _id = id;
            _time = time;
        }

        public IOwner Owner
        {
            get { return _owner; }
        }

        public int Id
        {
            get { return _id; }
        }

        public double Time
        {
            get { return _time; }
        }
    }
}