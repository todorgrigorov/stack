using System;

namespace Stack.Logging
{
    public interface ILogger
    {
        void Log(string text, LogType type = LogType.Info);
        void Log(Exception exception, LogType type = LogType.Error);
    }
}
