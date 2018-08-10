using Network.Values;

namespace Network.Protocol
{
    public abstract class UdpProtocol : IUdpProtocol
    {
        public event ProtocolConnectedHandler Connected;
        public event ProtocolDisconnectedHandler Disconnectd;
        public event ProtocolAuthorizeReceived AuthorizeReceived;
        public event ProtocolRequestReceived RequestReceived;
        public event ProtocolResponseReceived ResponseReceived;
        public event ProtocolErrorHandler ErrorReceived;

        public abstract bool Start(int port);
        public abstract void Stop();

        public abstract void Update();

        public abstract void Connect(string host, int port);
        public abstract void Disconnect(string address);
        
        public abstract void Request(string address, int id, IValue value);
        public abstract void Authorize(string address, int id, string name);
        public abstract void Ack(string address, int id, string text);
        public abstract void Fail(string address, int id, string text);
        public abstract void Error(string address, string value);

        protected void CallConnected(string address)
        {
            if (Connected != null)
            {
                Connected(this, address);
            }
        }

        protected void CallDisconnected(string address)
        {
            if (Disconnectd != null)
            {
                Disconnectd(this, address);
            }
        }

        protected void CallAuthorizeReceived(string address, int id, string name)
        {
            if (AuthorizeReceived != null)
            {
                AuthorizeReceived(this, address, id, name);
            }
        }

        protected void CallRequestReceived(string address, int id, IValue value)
        {
            if (RequestReceived != null)
            {
                RequestReceived(this, address, id, value);
            }
        }

        protected void CallResponseReceived(string address, int id, IValue value)
        {
            if (ResponseReceived != null)
            {
                ResponseReceived(this, address, id, value);
            }
        }
        protected void CallErrorReceived(string address, string errorCode)
        {
            if (ResponseReceived != null)
            {
                ErrorReceived(this, address, errorCode);
            }
        }
    }
}
