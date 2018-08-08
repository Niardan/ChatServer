using ChatServer.Callbacks;
using ChatServer.Values;

namespace ChatServer.Response
{
    public class AckResponse : Response
    {
        public AckResponse(IValue1 value) : base(value)
        {
        }

        public override void Receive(ICallbacks callbacks)
        {
            callbacks.Ack(_value);
        }
    }
}