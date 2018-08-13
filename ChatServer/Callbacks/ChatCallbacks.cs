using System;

namespace ChatServer.Callbacks
{
    public class ChatCallbacks : Network.Callbacks.Callbacks
    {
        private readonly string _nameSender;
        private readonly string _nameRecipient;
        private readonly string _message;

        public ChatCallbacks(string nameSender, string nameRecipient, string message)
        {
            _nameSender = nameSender;
            _nameRecipient = nameRecipient;
            _message = message;
        }

        public override void Ack(string value)
        {
        }

        public override void Fail(string value)
        {
            Console.WriteLine("Cooбщение: {0}, от пользователя {1}, не доставлено пользователю {2}, по причине: {3}", _message, _nameSender, _nameRecipient, value);
        }
    }
}