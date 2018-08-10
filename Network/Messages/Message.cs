using MessagePack;

namespace Network.Messages
{
    [MessagePackObject]
    public class Message : IMessage
    {
        public Message(int id, string typeMessage)
        {
            Id = id;
            TypeMessage = typeMessage;
        }
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string TypeMessage { get; set; }
    }
}