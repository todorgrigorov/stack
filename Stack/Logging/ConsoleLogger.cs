using System;
using System.Diagnostics;

namespace Stack.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string text, LogType type = LogType.Info)
        {
            Assure.NotEmpty(text, nameof(text));
            Trace.WriteLine(text, type.ToString());
        }
        public void Log(Exception exception, LogType type = LogType.Error)
        {
            Assure.NotNull(exception, nameof(exception));
            Trace.WriteLine(exception.Message, type.ToString());
            Trace.WriteLine(exception.StackTrace);
        }
    }
}
