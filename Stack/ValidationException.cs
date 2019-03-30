using System;

namespace Stack
{
    public class ValidationException : Exception
    {
        public ValidationException()
        {
        }
        public ValidationException(string message) 
            : base(message)
        {
        }
        public ValidationException(ValidationError error)
            : base(error.Message)
        {
            Error = error;
        }
        public ValidationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        public ValidationError Error { get; set; }
    }
}