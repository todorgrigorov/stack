using System;

namespace Stack.Data
{
    public class EntityNotUniqueException : Exception
    {
        public EntityNotUniqueException()
        {
        }
        public EntityNotUniqueException(string message)
            : base(message)
        {
        }
        public EntityNotUniqueException(NotUniqueError error)
            : base(error.Message)
        {
            Error = error;
        }
        public EntityNotUniqueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NotUniqueError Error { get; set; }
    }
}
