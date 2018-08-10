using MessagePack;
using Network.Values;

namespace Network.Messages
{
    [MessagePackObject]
    public class RequestMessage : Message
    {
        public RequestMessage(int id) : base(id, "request")
        {
           // MessageValue = messageValue;
        }

        [Key(2)]
        public IValue MessageValue { get; set; }
    }
}