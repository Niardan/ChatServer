using MessagePack;

namespace ChatServer.Messages
{
    public class AuthorizeMessage : Message
    {
        public AuthorizeMessage(int id, string name) : base(id, "authorize")
        {
            Name = name;
        }

        [Key(2)]
        public string Name { get; set; }
    }
}