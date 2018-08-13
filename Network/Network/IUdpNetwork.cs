using Network.Callbacks;
using Network.Owner;
using Network.Values;

namespace Network.Network
{
    public delegate void NetworkConnectedHandler(IUdpNetwork network, IOwner owner);
    public delegate void NetworkDisconnectedHandler(IUdpNetwork network, IOwner owner);
    public delegate void NetworkRequestReceivedHandler(IUdpNetwork network, IOwner owner, IValue request, ICallbacks callbacks);

    public interface IUdpNetwork
    {
        event NetworkConnectedHandler Connected;
        event NetworkDisconnectedHandler Disconnected;
        event NetworkRequestReceivedHandler RequestReceived;

        bool Start(int port);
        void Stop();

        void Update();

        void Request(IOwner owner, IValue value, ICallbacks callbacks);

        void Disconnect(IOwner owner);
    }
}
