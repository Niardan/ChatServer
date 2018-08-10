using System;

namespace Network.Messages
{
    [Serializable]
    public class Message : IMessage
    {
        public Message(int id, string typeMessage)
        {
            Id = id;
            TypeMessage = typeMessage;
        }
    
        public int Id { get; set; }
      
        public string TypeMessage { get; set; }
    }
}