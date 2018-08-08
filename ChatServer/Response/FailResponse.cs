using ChatServer.Callbacks;
using ChatServer.Values;

namespace ChatServer.Response
{
    public class FailResponse : Response
    {
        public FailResponse(string value) : base(value)
        {
        }

        public override void Receive(ICallbacks callbacks)
        {
            callbacks.Fail(_value);
        }
    }
}