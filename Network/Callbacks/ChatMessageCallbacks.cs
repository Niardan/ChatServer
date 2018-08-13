using System;

namespace Network.Callbacks
{
    public class ChatMessageCallbacks : Callbacks
    {
        private readonly string _sendAddress;
        private readonly string _name;
        private readonly string _message;

        public ChatMessageCallbacks(string sendAddress, string name, string message)
        {
            _sendAddress = sendAddress;
            _name = name;
            _message = message;
        }

        public override void Ack(string value)
        {
            Console.WriteLine("");
        }

        public override void Fail(string value)
        {
            Console.WriteLine("Cooбщение: {0}, от пользователя {1}, не доставлено по адресу {2}, по причине: {3}", _message, _name, _sendAddress,  value);
        }
    }
}