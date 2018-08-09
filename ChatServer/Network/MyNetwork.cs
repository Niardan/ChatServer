using System.Collections;
using System.Collections.Generic;
using ChatServer.Callbacks;
using ChatServer.Owner;
using ChatServer.Protocol;
using ChatServer.Values;

namespace ChatServer.Network
{
    public class MyNetwork : UdpNetwork
    {
        private readonly IUdpProtocol _protocol;
        private readonly IDictionary<string, IOwner> _toOwner = new Dictionary<string, IOwner>();
        private readonly ICollection<string> _toAutorized = new HashSet<string>();
        private readonly ICollection<string> _authorizing = new HashSet<string>();

        private readonly IDictionary<long, ICallbacks> _callbacks = new Dictionary<long, ICallbacks>();
        private long _countRequest = 0;

        private readonly string _alreadyAuthorized = "alreadyAuthorized";
        private readonly string _notAuthorized = "notAuthorized";
        private readonly string _alreadyResponse = "alreadyResponse";


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
            if (!_authorizing.Contains(address)||!_toAutorized.Contains(address))
            {
                IOwner owner = _toOwner[address];
                CallAuthorizeReceived(owner, new UdpNetworkRequestCallback(protocol, address, id));
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

        private void OnProtocolResponseReceived(IUdpProtocol protocol, string address, long id, IValue1 message)
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

        private void OnProtocolRequestReceived(IUdpProtocol protocol, string address, long id, IValue1 message)
        {
            if (_toAutorized.Contains(address))
            {
                IOwner owner = _toOwner[address];
                CallAuthorizeReceived(owner, new UdpNetworkRequestCallback(protocol, address, id));
            }
            else
            {
                protocol.Error(address, _alreadyAuthorized);
            }
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Request(IOwner owner, IValue1 value, ICallbacks callbacks)
        {
            throw new System.NotImplementedException();
        }

        public override void Disconnect(IOwner owner)
        {
            throw new System.NotImplementedException();
        }
    }

    public class RequestInfo
    {

    }
}