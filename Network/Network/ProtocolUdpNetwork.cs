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


        private readonly int _timeout;

        private readonly IDictionary<long, Tuple<ICallbacks, RequestInfo>> _callbacks = new Dictionary<long, Tuple<ICallbacks, RequestInfo>>();
        private int _countRequest = 0;

        private readonly string _timeoutMessage = "timeout";
        private readonly string _unknownFormat = "unknownFormat";

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
                CallDisconnected(owner);
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
                _protocol.Error(address, _unknownFormat);
            }
        }

        private void OnProtocolRequestReceived(IUdpProtocol protocol, string address, int id, IValue message)
        {
            IOwner owner = _toOwner[address];
            CallRequestReceived(owner, message, new UdpNetworkRequestCallback(protocol, address, id));
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
                IOwner owner = requestInfo.Owner;
                callback.Fail(_timeoutMessage);
            }
        }

        public override void Request(IOwner owner, IValue value, ICallbacks callbacks)
        {
            int id = _countRequest;
            var requestInfo = new RequestInfo(owner, id, _now.Get);
            _sent.AddLast(requestInfo);
            _callbacks.Add(id, new Tuple<ICallbacks, RequestInfo>(callbacks, requestInfo));
            _protocol.Request(owner.Id, id, value);
            _countRequest++;
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