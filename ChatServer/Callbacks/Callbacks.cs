using ChatServer.Values;

namespace ChatServer.Callbacks
{
    public abstract class Callbacks : ICallbacks
    {
        public abstract void Ack(string value);
        public abstract void Fail(string value);
    }
}