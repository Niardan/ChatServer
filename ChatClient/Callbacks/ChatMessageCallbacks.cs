using System.Windows.Forms;

namespace ChatClient.Callbacks
{
    public delegate void ChatSendFailHandler(string reason);
    public class ChatMessageCallbacks :Network.Callbacks.Callbacks
    {
        private readonly ChatSendFailHandler _chatSendFail;
        public ChatMessageCallbacks(ChatSendFailHandler chatSendFail)
        {
            _chatSendFail = chatSendFail;
        }

        public override void Ack(string value)
        {
        }

        public override void Fail(string value)
        {
            _chatSendFail.Invoke(value);
        }
    }
}