namespace ChatClient.Callbacks
{
    public class ChatMessageCallbacks : Network.Callbacks.Callbacks
    {
        private readonly ChatClient _chatClient;
        public ChatMessageCallbacks(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public override void Ack(string value)
        {
        }

        public override void Fail(string value)
        {
            _chatClient.FailChat(value);
        }
    }
}