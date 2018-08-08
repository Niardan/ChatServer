using ChatServer.Network;
using ChatServer.Values;

namespace ChatServer.Callbacks
{
    public class UdpNetworkRequestCallback : Callbacks
    {
        private readonly ServerUdpNetwork _network;
        private readonly string _peer;
        private readonly long _id;

        public UdpNetworkRequestCallback(ServerUdpNetwork network, string peer, long id)
        {
            _network = network;
            _peer = peer;
            _id = id;
        }

        public override void Ack(string value)
        {
            _network.Ack(_peer, _id, value);
        }

        public override void Fail(string value)
        {
            _network.Fail(_peer, _id, value);
        }
    }
}