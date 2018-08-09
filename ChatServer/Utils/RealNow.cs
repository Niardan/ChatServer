using System;

namespace ChatServer.Utils
{
    public class RealNow : Now
    {
        private readonly DateTime _start = new DateTime(1970, 1, 1);

        public override double Get
        {
            get { return DateTime.UtcNow.Subtract(_start).TotalMilliseconds; }
        }
    }
}