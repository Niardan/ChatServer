using ChatServer.Callbacks;
using ChatServer.Values;

namespace ChatServer.Response
{
    public abstract class Response : IResponse
    {
        protected readonly string _value;

        protected Response(string value)
        {
            _value = value;
        }

        public abstract void Receive(ICallbacks callbacks);
    }
}