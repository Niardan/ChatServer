﻿using System;

namespace Network.Messages
{
    [Serializable]
    public class ErrorMessage : Message
    {
        public ErrorMessage(int id, string error) : base(id, "error")
        {
            Error = error;
        }

        public string Error { set; get; }
    }
}