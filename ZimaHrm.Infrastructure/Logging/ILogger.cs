using System;

namespace ZimaHrm.Core.Infrastructure.Logging
{
    public interface ILogger
    {
        void Debug(string message);

        void Debug(string message, object data);

        void Error(Exception exception);

        void Error(Exception exception, object data);

        void Error(string message);

        void Error(string message, object data);

        void Fatal(Exception exception);

        void Fatal(Exception exception, object data);

        void Information(string message);

        void Information(string message, object data);

        void Warning(string message);
        void Warning(string message, object data);

        void LogFile(string message);
        void LogFile(Exception exception);

    }
}
