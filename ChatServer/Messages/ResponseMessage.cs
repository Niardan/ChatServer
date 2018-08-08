using ChatServer.Values;
using MessagePack;

namespace ChatServer.Messages
{
    [MessagePackObject]
    public class ResponseMessage : Message
    {
        public ResponseMessage(int id,  IValue1 answer) : base(id, "request")
        {
            Answer = answer;
        }
        [Key(2)]
        public IValue1 Answer { get; set; }
    }
}