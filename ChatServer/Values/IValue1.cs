namespace ChatServer.Values
{
    [MessagePack.Union(0, typeof(ChatValue1))]
    [MessagePack.Union(0, typeof(ResponseValue))]
    public interface IValue1
    {
        
    }
}