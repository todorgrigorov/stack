using System;

namespace Stack
{
    public class AssemblyLoadingException : Exception
    {
        public AssemblyLoadingException()
        {
        }
        public AssemblyLoadingException(string message) 
            : base(message)
        {
        }
        public AssemblyLoadingException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
