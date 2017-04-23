namespace Hermes.DataObjects
{
    public class HermesApiExceptionDto
    {
        public string Message { get; private set; }
        public string StackTrace { get; private set; }

        public HermesApiExceptionDto(string message, string stackTrace)
        {
            Message = message;
            StackTrace = stackTrace;
        }
    }
}
