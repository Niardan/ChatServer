using System;

namespace Network.Values
{
    [Serializable]
    public class ResponseValue : Value
    {
        public ResponseValue(string answer, string text)
        {
            Answer = answer;
            Text = text;
        }

        public string Answer { get; set; }

        public string Text { get; set; }
    }
}