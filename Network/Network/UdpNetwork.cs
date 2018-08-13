using Network.Callbacks;
using Network.Owner;
using Network.Values;

namespace Network.Network
{
    public abstract class UdpNetwork : IUdpNetwork
    {
        public event NetworkConnectedHandler Connected;
        public event NetworkDisconnectedHandler Disconnected;
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

        protected void CallConnected(IOwner owner)
        {
            if (Connected != null)
            {
                Connected(this, owner);
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