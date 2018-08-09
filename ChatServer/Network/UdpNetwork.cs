using ChatServer.Callbacks;
using ChatServer.Owner;
using ChatServer.Values;

namespace ChatServer.Network
{
    public abstract class UdpNetwork : IUdpNetwork

    {
        public event NetworkDisconnectedHandler Disconnected;
        public event NetworkAuthorizeReceivedHandler AuthorizeReceived;
        public event NetworkRequestReceivedHandler RequestReceived;

        public abstract bool Start(int port);
        public abstract void Stop();
        public abstract void Update();
        public abstract void Request(IOwner owner, IValue1 value, ICallbacks callbacks);
        public abstract void Disconnect(IOwner owner);

        protected void CallDisconnected(IOwner owner)
        {
            if (Disconnected != null)
            {
                Disconnected(this, owner);
            }
        }

        protected void CallAuthorizeReceived(IOwner owner, ICallbacks callbacks)
        {
            if (AuthorizeReceived != null)
            {
                AuthorizeReceived(this, owner, callbacks);
            }
        }

        protected void CallRequestReceived(IOwner owner, IValue1 request, ICallbacks callbacks)
        {
            if (RequestReceived != null)
            {
                RequestReceived(this, owner, request, callbacks);
            }
        }
    }

}