 using System;
using System.Net;

namespace Stack.Web
{
    public class RestRequestException : Exception
    {
        public RestRequestException()
        {
        }
        public RestRequestException(string message) 
            : base(message)
        {
        }
        public RestRequestException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
        public RestRequestException(string message, HttpStatusCode status)
            : base(message)
        {
            Status = status;
        }
        public RestRequestException(string message, Exception innerException, HttpStatusCode status)
            : base(message, innerException)
        {
            Status = status;
        }

        public HttpStatusCode Status { get; set; }
    }
}