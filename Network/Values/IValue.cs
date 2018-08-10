namespace Network.Values
{
    [MessagePack.Union(0, typeof(ChatValue))]
    [MessagePack.Union(0, typeof(ResponseValue))]
    public interface IValue
    {
        
    }
}