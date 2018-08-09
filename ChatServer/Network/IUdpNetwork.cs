using ChatServer.Callbacks;
using ChatServer.Owner;
using ChatServer.Values;

namespace ChatServer.Network
{
    public delegate void NetworkDisconnectedHandler(IUdpNetwork network, IOwner owner);
    public delegate void NetworkAuthorizeReceivedHandler(IUdpNetwork network, IOwner owner, ICallbacks callbacks);
    public delegate void NetworkRequestReceivedHandler(IUdpNetwork network, IOwner owner, IValue1 request, ICallbacks callbacks);

    public interface IUdpNetwork
    {
        event NetworkDisconnectedHandler Disconnected;
        event NetworkAuthorizeReceivedHandler AuthorizeReceived;
        event NetworkRequestReceivedHandler RequestReceived;

        bool Start(int port);
        void Stop();

        void Update();

        void Request(IOwner owner, IValue1 value, ICallbacks callbacks);

        void Disconnect(IOwner owner);
    }
}
