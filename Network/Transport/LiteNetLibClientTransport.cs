using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Network.Transport
{
    public class LiteNetLibClientTransport : UdpTransport
    {
        private readonly EventBasedNetListener _listener = new EventBasedNetListener();
        private readonly NetManager _netManager;

        private readonly IDictionary<string, NetPeer> _toPeer = new Dictionary<string, NetPeer>();
        private readonly IDictionary<NetPeer, string> _toAddress = new Dictionary<NetPeer, string>();

        public LiteNetLibClientTransport(int maxConnections, string connectKey)
        {
            _netManager = new NetManager(_listener, maxConnections, connectKey);
        }

        public override bool Start(int port)
        {
            if (_netManager.Start())
            {
                _listener.PeerConnectedEvent += OnPeerConnected;
                _listener.PeerDisconnectedEvent += OnPeerDisconnected;
                _listener.NetworkReceiveEvent += OnNetworkReceive;
                _listener.NetworkErrorEvent += OnNetworkErrorEvent;
                return true;
            }

            return false;
        }

        public override void Stop()
        {
            _listener.PeerConnectedEvent -= OnPeerConnected;
            _listener.PeerDisconnectedEvent -= OnPeerDisconnected;
            _listener.NetworkReceiveEvent -= OnNetworkReceive;
            _listener.NetworkErrorEvent -= OnNetworkErrorEvent;

            _netManager.Stop();
        }

        public override void Update()
        {
            _netManager.PollEvents();
        }

        public override void Connect(string host, int port)
        {
            string address = host + port;
            NetPeer peer = _netManager.Connect(host, port);
            peer.Tag = address;
            _toPeer.Add(address, peer);
        }

        public override void Disconnect(string address)
        {
            NetPeer netPeer;
            if (_toPeer.TryGetValue(address, out netPeer))
            {
                _netManager.DisconnectPeer(netPeer);
            }
        }

        public override void Send(string address, byte[] bytes)
        {
            NetPeer netPeer;
            if (_toPeer.TryGetValue(address, out netPeer))
            {
                netPeer.Send(bytes, SendOptions.ReliableOrdered);
            }
        }

        private void OnPeerConnected(NetPeer peer)
        {
            if (!_toAddress.ContainsKey(peer))
            {
                string address = ToAddress(peer);
                _toAddress.Add(peer, address);
                CallConnected(address);
            }
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            string address;
            if (_toAddress.TryGetValue(peer, out address))
            {
                _toPeer.Remove(address);
                _toAddress.Remove(peer);
                CallDisconnected(address);
            }
            else
            {
                address = ToAddress(peer);
                if (_toPeer.ContainsKey(address))
                {
                    _toPeer.Remove(address);
                    CallDisconnected(address);
                }
            }

            peer.Tag = null;
        }

        private void OnNetworkReceive(NetPeer peer, NetDataReader reader)
        {
            string address;
            if (_toAddress.TryGetValue(peer, out address))
            {
                CallReceived(address, reader.Data);
            }
        }
        
        private void OnNetworkErrorEvent(NetEndPoint endPoint, int socketErrorCode)
        {
            CallError(ToAddress(endPoint), socketErrorCode);
        }
        
        private string ToAddress(NetPeer peer)
        {
            object tag = peer.Tag;
            if (tag != null)
            {
                return (string)tag;
            }
            return string.Empty;
        }

        private string ToAddress(NetEndPoint endPoint)
        {
            if (endPoint == null)
            {
                return string.Empty;
            }

            string host = endPoint.Host;
            if (host == null)
            {
                host = string.Empty;
            }
            return host + endPoint.Port;
        }
    }
}