using MessagePack;

namespace Network.Messages
{
    [MessagePackObject]
    public class ErrorMessage : Message
    {
        public ErrorMessage(int id, string error) : base(id, "error")
        {
            Error = error;
        }
        [Key(2)]
        public string Error { set; get; }
    }
}