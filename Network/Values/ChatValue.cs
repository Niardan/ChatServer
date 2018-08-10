using MessagePack;

namespace Network.Values
{
    [MessagePackObject]
    public class ChatValue : IValue
    {
        [Key(0)]
        public string Name { set; get; }

        [Key(1)]
        public string Message { set; get; }
    }
}