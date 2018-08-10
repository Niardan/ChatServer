using MessagePack;
using Network.Values;

namespace Network.Messages
{
    [MessagePackObject]
    public class ResponseMessage : Message
    {
        public ResponseMessage(int id, IValue answer) : base(id, "request")
        {
            Answer = answer;
        }
        [Key(2)]
        public IValue Answer { get; set; }
    }
}