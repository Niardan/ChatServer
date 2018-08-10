
using System;
using Network.Values;

namespace Network.Messages
{
    [Serializable]
    public class RequestMessage : Message
    {
        public RequestMessage(int id, IValue messageValue) : base(id, "request")
        {
            MessageValue = messageValue;
        }

     
        public IValue MessageValue { get; set; }
    }
}