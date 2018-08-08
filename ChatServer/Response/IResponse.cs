using ChatServer.Callbacks;

namespace ChatServer.Response
{
    public interface IResponse
    {
        void Receive(ICallbacks callbacks);
    }
}