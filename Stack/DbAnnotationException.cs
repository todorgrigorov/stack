using System;

namespace Stack
{
    public class DbAnnotationException : Exception
    {
        public DbAnnotationException()
        {
        }
        public DbAnnotationException(string message) 
            : base(message)
        {
        }
        public DbAnnotationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
