namespace Network.Callbacks
{
    public interface ICallbacks
    {
        void Ack(string value);
        void Fail(string value);
    }
}