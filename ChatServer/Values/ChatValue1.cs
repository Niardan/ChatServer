using MessagePack;

namespace ChatServer.Values
{
    [MessagePackObject]
    public class ChatValue1 : IValue1
    {
        [Key(0)]
        private string Name { set; get; }
        [Key(1)]
        private string Message { set; get; }
    }
}