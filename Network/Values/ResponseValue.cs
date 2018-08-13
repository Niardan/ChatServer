using System;

namespace Network.Values
{
    [Serializable]
    public class ResponseValue : Value
    {
        private readonly string _answer;
        private readonly string _text;

        public ResponseValue(string answer, string text)
        {
            _answer = answer;
            _text = text;
        }

        public string Answer { get { return _answer; } }

        public string Text { get { return _text; } }
    }
}