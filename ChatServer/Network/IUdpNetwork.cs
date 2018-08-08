using ChatServer.Callbacks;
using ChatServer.Owner;
using ChatServer.Response;
using ChatServer.Values;

namespace ChatServer.Network
{
    public delegate void NetworkDisconnectedHandler(IUdpNetwork network, string destination, string peer, ICallbacks callbacks);
    public delegate void NetworkAuthorizeReceivedHandler(IUdpNetwork network, string destination, string peer, string token, ICallbacks callbacks);
    public delegate void NetworkRequestReceivedHandler(IUdpNetwork network, string destination, string peer, IValue1 request, ICallbacks callbacks);
    public delegate void NetworkResponseReceivedHandler(IUdpNetwork network, long id, string destination, IResponse response);

    public interface IUdpNetwork
    {
        event NetworkDisconnectedHandler Disconnected;
        event NetworkAuthorizeReceivedHandler AuthorizeReceived;
        event NetworkRequestReceivedHandler RequestReceived;
        event NetworkResponseReceivedHandler ResponseReceived;

        bool Start(int port);
        void Stop();

        void Update();

        void Request(IOwner owner, string destination, string peer, IValue1 value);

        void Disconnect(string destination);
    }
}
