using ChatServer.Values;

namespace ChatServer.Callbacks
{
    public interface ICallbacks
    {
        void Ack(string value);
        void Fail(string value);
    }
}