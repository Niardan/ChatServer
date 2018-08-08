using MessagePack;

namespace ChatServer.Messages
{
    [MessagePackObject]
    public class Message : IMessage
    {
        public Message(long id, string typeMessage)
        {
            Id = id;
            TypeMessage = typeMessage;
        }
        [Key(0)]
        public long Id { get; set; }
        [Key(1)]
        public string TypeMessage { get; set; }
    }
}