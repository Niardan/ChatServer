using MessagePack;

namespace Network.Values
{
    public class ResponseValue : IValue
    {
        public ResponseValue(string answer, string text)
        {
            Answer = answer;
            Text = text;
        }

        [Key(0)]
        public string Answer { get; set; }
        [Key(1)]
        public string Text { get; set; }
    }
}