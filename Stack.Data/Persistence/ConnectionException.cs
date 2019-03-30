﻿using System;

namespace Stack.Data.Persistence
{
    public class ConnectionException : Exception
    {
        public ConnectionException()
        {
        }
        public ConnectionException(string message)
            : base(message)
        {
        }
        public ConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
