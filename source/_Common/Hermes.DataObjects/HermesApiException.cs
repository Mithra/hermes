using System;
using System.Runtime.Serialization;

namespace Hermes.DataObjects
{
    [Serializable]
    public class HermesApiException : Exception
    {
        public string ExceptionCallback { get; private set; }

        public HermesApiException()
        {
        }

        public HermesApiException(string message, string exceptionCallback = null)
            : base(message)
        {
            ExceptionCallback = exceptionCallback;
        }

        public HermesApiException(string message, string exceptionCallback, Exception innerException)
            : base(message, innerException)
        {
            ExceptionCallback = exceptionCallback;
        }

        protected HermesApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
