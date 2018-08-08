using ChatServer.Values;

namespace ChatServer.Callbacks
{
    public abstract class Callbacks : ICallbacks
    {
        public abstract void Ack(IValue1 value);
        public abstract void Fail(IValue1 value);
    }
}