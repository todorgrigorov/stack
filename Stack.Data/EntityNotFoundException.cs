using System;

namespace Stack.Data
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }
        public EntityNotFoundException(string message)
            : base(message)
        {
        }
        public EntityNotFoundException(NotFoundError error)
            : base(error.Message)
        {
            Error = error;
        }
        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NotFoundError Error { get; set; }
    }
}
