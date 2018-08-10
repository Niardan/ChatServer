using Network.Callbacks;
using Network.Owner;
using Network.Values;

namespace Network.Network
{
    public abstract class UdpNetwork : IUdpNetwork

    {
        public event NetworkDisconnectedHandler Disconnected;
        public event NetworkAuthorizeReceivedHandler AuthorizeReceived;
        public event NetworkRequestReceivedHandler RequestReceived;

        public abstract bool Start(int port);
        public abstract void Stop();
        public abstract void Update();
        public abstract void Request(IOwner owner, IValue value, ICallbacks callbacks);
        public abstract void Disconnect(IOwner owner);

        protected void CallDisconnected(IOwner owner)
        {
            if (Disconnected != null)
            {
                Disconnected(this, owner);
            }
        }

        protected void CallAuthorizeReceived(IOwner owner, string name, ICallbacks callbacks)
        {
            if (AuthorizeReceived != null)
            {
                AuthorizeReceived(this, owner, name, callbacks);
            }
        }

        protected void CallRequestReceived(IOwner owner, IValue request, ICallbacks callbacks)
        {
            if (RequestReceived != null)
            {
                RequestReceived(this, owner, request, callbacks);
            }
        }
    }

}