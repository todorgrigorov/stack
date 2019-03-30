using System;

namespace Stack.Data
{
    public class DbObjectException : Exception
    {
        public DbObjectException()
        {
        }
        public DbObjectException(string message) 
            : base(message)
        {
        }
        public DbObjectException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
