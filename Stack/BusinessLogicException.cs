using System;
using System.Runtime.Serialization;

namespace Stack
{
    public class BusinessLogicException : Exception
    {
        public BusinessLogicException()
        {
        }
        public BusinessLogicException(string message, string domain)
            : base(message)
        {
            Domain = domain;
        }
        public BusinessLogicException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #region Protected members
        protected BusinessLogicException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        public string Domain { get; set; }
    }
}
