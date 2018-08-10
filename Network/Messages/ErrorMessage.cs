using MessagePack;

namespace Network.Messages
{
    [MessagePackObject]
    public class ErrorMessage : IMessage
    {
        public ErrorMessage(int id, string error) //: base(id, "error")
        {
            Error = error;
        }
        [Key(2)]
        public string Error { set; get; }

        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string TypeMessage { get; set; }
    }
}