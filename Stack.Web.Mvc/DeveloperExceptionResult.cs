namespace Stack.Web.Mvc
{
    public class DeveloperExceptionResult
    {
        public DeveloperExceptionResult(string exception, string trace)
        {
            Exception = exception;
            Trace = trace;
        }

        public string Exception { get; set; }
        public string Trace { get; set; }
    }
}
