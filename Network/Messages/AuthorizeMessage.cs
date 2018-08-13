using System;

namespace Network.Messages
{
    [Serializable]
    public class AuthorizeMessage : Message
    {
        public AuthorizeMessage(int id, string name) : base(id, "authorize")
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}