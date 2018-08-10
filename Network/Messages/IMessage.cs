namespace Network.Messages
{
    //[MessagePack.Union(0, typeof(RequestMessage))]
    //[MessagePack.Union(1, typeof(AuthorizeMessage))]
    //[MessagePack.Union(2, typeof(ResponseMessage))]
    [MessagePack.Union(0, typeof(ErrorMessage))]
    public interface IMessage
    {
        //int Id { get; }
        //string TypeMessage { get; }
    }
}