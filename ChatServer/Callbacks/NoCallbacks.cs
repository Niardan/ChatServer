using ChatServer.Values;

namespace ChatServer.Callbacks
{
    public class NoCallbacks : Callbacks
    {
        public override void Ack(IValue1 value)
        {
        }

        public override void Fail(IValue1 value)
        {
        }
    }
}