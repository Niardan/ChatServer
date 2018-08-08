using ChatServer.Values;
using MessagePack;

namespace ChatServer.Messages
{
    [MessagePackObject]
    public class RequestMessage : Message
    {
        public RequestMessage(int id, string name, IValue1 value) : base(id, "request")
        {
            Value = value;
        }

        [Key(2)]
        public IValue1 Value { get; set; }
    }
}