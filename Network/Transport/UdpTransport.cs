namespace Network.Transport
{
    public abstract class UdpTransport : IUdpTransport
    {
        public event TransportConnectedHandler Connected;
        public event TransportDisconnectedHandler Disconnected;
        public event TransportReceivedHandler Received;
        public event TransportErrorHandler Error;

        public abstract bool Start(int port);
        public abstract void Stop();

        public abstract void Update();

        public abstract void Connect(string host, int port);
        public abstract void Disconnect(string address);
        public abstract void Send(string address, byte[] bytes);

        protected void CallConnected(string address)
        {
            if (Connected != null)
            {
                Connected(this, address);
            }
        }

        protected void CallDisconnected(string address)
        {
            if (Disconnected != null)
            {
                Disconnected(this, address);
            }
        }

        protected void CallReceived(string address, byte[] bytes)
        {
            if (Received != null)
            {
                Received(this, address, bytes);
            }
        }

        protected void CallError(string address, int errorCode)
        {
            if (Error != null)
            {
                Error(this, address, errorCode);
            }
        }
    }
}
