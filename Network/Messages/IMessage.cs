namespace Network.Messages
{
    public interface IMessage
    {
        int Id { get; }
        string TypeMessage { get; }
    }
}