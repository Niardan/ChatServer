using MessagePack;
using Network.Values;

namespace Network.Messages
{
    [MessagePackObject]
    public class RequestMessage : Message
    {
        public RequestMessage(long id, IValue value) : base(id, "request")
        {
            Value = value;
        }

        [Key(2)]
        public IValue Value { get; set; }
    }
}