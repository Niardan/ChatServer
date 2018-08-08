namespace ChatServer.Transport
{
    public delegate void TransportConnectedHandler(IUdpTransport transport, string address);
    public delegate void TransportDisconnectedHandler(IUdpTransport transport, string address);
    public delegate void TransportReceivedHandler(IUdpTransport transport, string address, byte[] bytes);
    public delegate void TransportErrorHandler(IUdpTransport transport, string address, int errorCode);

    public interface IUdpTransport
    {
        event TransportConnectedHandler Connected;
        event TransportDisconnectedHandler Disconnected;
        event TransportReceivedHandler Received;
        event TransportErrorHandler Error;

        bool Start(int port);
        void Stop();

        void Update();

        void Connect(string host, int port);
        void Disconnect(string address);
        void Send(string address, byte[] bytes);
    }
}
