using System;
using Stack.Logging;

namespace Stack.Data.Logging
{
    public class DatabaseLogger : ILogger
    {
        public void Log(string text, LogType type = LogType.Info)
        {
            Assure.NotEmpty(text, nameof(text));
            Log(new LogMessage()
            {
                Message = text,
                Type = type
            });
        }
        public void Log(Exception exception, LogType type = LogType.Error)
        {
            Assure.NotNull(exception, nameof(exception));
            Log(new LogMessage()
            {
                Message = $"{exception.Message.Trim()} -> {exception.StackTrace.Trim()}",
                Type = type
            });
        }

        #region Private members
        private void Log(LogMessage message)
        {
            if (message.Message.Length > LogMessage.MaxLogLength)
            {
                message.Message = message.Message.Substring(0, LogMessage.MaxLogLength);
            }

            if (Database.Persister.IsConnected)
            {
                Dao.For<LogMessage>().Create(message);
            }
            else
            {
                Database.InConnection(() => Dao.For<LogMessage>().Create(message));
            }
        }
        #endregion
    }
}
