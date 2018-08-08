using ChatServer.Messages;
using ChatServer.Values;

namespace ChatServer.Protocol
{
    public abstract class UdpProtocol : IUdpProtocol
    {
        public event ProtocolConnectedHandler Connected;
        public event ProtocolDisconnectedHandler Disconnectd;
        public event ProtocolAuthorizeReceived AuthorizeReceived;
        public event ProtocolRequestReceived RequestReceived;
        public event ProtocolResponseReceived ResponseReceived;

        public abstract bool Start(int port);
        public abstract void Stop();

        public abstract void Update();

        public abstract void Connect(string host, int port);
        public abstract void Disconnect(string address);
        
        public abstract void Request(string address, long id, IMessage value);
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

        protected void CallAuthorizeReceived(string address, string name)
        {
            if (AuthorizeReceived != null)
            {
                AuthorizeReceived(this, address, name);
            }
        }

        protected void CallRequestReceived(string address, long id, IValue1 value)
        {
            if (RequestReceived != null)
            {
                RequestReceived(this, address, id, value);
            }
        }

        protected void CallResponseReceived(string address, long id, IValue1 value)
        {
            if (ResponseReceived != null)
            {
                ResponseReceived(this, address, id, value);
            }
        }
    }
}
