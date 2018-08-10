using Network.Protocol;

namespace Network.Callbacks
{
    public class UdpNetworkRequestCallback : Callbacks
    {
        private readonly IUdpProtocol _protocol;
        private readonly string _address;
        private readonly int _id;

        public UdpNetworkRequestCallback(IUdpProtocol protocol, string peer, int id)
        {
            _protocol = protocol;
            _address = peer;
            _id = id;
        }

        public override void Ack(string value)
        {
            _protocol.Ack(_address, _id, value);
        }

        public override void Fail(string value)
        {
            _protocol.Fail(_address, _id, value);
        }
    }
}