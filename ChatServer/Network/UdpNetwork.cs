using ChatServer.Callbacks;
using ChatServer.Owner;
using ChatServer.Response;
using ChatServer.Values;

namespace ChatServer.Network
{
    public abstract class UdpNetwork : IUdpNetwork

    {
    public event NetworkDisconnectedHandler Disconnected;
    public event NetworkAuthorizeReceivedHandler AuthorizeReceived;
    public event NetworkRequestReceivedHandler RequestReceived;
    public event NetworkResponseReceivedHandler ResponseReceived;

    public abstract bool Start(int port);
    public abstract void Stop();
    public abstract void Update();
    public abstract void Request(IOwner owner, string destination, string peer, IValue1 value);
    public abstract void Disconnect(string destination);

    protected void CallDisconnected(string destination, string peer, ICallbacks callbacks)
    {
        if (Disconnected != null)
        {
            Disconnected(this, destination, peer, callbacks);
        }
    }

    protected void CallAuthorizeReceived(string destination, string peer, string tocken, ICallbacks callbacks)
    {
        if (AuthorizeReceived != null)
        {
            AuthorizeReceived(this, destination, peer, tocken, callbacks);
        }
    }

    protected void CallRequestReceived(string destination, string peer, IValue1 request, ICallbacks callbacks)
    {
        if (RequestReceived != null)
        {
            RequestReceived(this, destination, peer, request, callbacks);
        }
    }

    protected void CallResponseReceived(long id, string destination, IResponse response)
    {
        if (ResponseReceived != null)
        {
            ResponseReceived(this, id, destination, response);
        }
    }
    }
}