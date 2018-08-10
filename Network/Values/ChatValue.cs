using System;

namespace Network.Values
{
    [Serializable]
    public class ChatValue : IValue
    {
        private string _name;
        private string _message;
        public ChatValue(string name, string message)
        {
            _name = name;
            _message = message;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Message
        {
            get { return _message; }
        }
    }
}