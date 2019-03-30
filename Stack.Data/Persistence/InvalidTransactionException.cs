using System;

namespace Stack.Data.Persistence
{
    public class InvalidTransactionException : Exception
    {
        public InvalidTransactionException()
        {
        }
        public InvalidTransactionException(string message)
            : base(message)
        {
        }
        public InvalidTransactionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
