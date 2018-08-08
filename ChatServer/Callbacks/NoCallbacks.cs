using ChatServer.Values;

namespace ChatServer.Callbacks
{
    public class NoCallbacks : Callbacks
    {
        public override void Ack(string value)
        {
        }

        public override void Fail(string value)
        {
        }
    }
}