using System;
using Network.Values;

namespace Network.Messages
{
    [Serializable]
    public class ResponseMessage : Message
    {
        public ResponseMessage(int id, IValue answer) : base(id, "response")
        {
            Answer = answer;
        }
       
        public IValue Answer { get; set; }
    }
}