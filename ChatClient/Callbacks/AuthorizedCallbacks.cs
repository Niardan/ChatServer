using System.Windows.Forms;

namespace ChatClient.Callbacks
{
    public class AuthorizedCallbacks :Network.Callbacks.Callbacks
    {
        public override void Ack(string value)
        {
            MessageBox.Show("Authorized Success!");
        }

        public override void Fail(string value)
        {
            MessageBox.Show("Authorized Fail!");
        }
    }
}