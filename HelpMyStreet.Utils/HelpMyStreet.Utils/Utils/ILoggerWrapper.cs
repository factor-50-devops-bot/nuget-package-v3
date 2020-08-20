using System;

namespace HelpMyStreet.Utils.Utils
{
    public interface ILoggerWrapper<T>
    {
        void LogError(string message, Exception exception);
        void LogError(string message, Exception exception, object request);
        void LogErrorAndNotifyNewRelic(string message, Exception exception);
        void LogErrorAndNotifyNewRelic(string message, Exception exception, object request);
        void LogWarning(string message, Exception exception);
        void LogWarning(string message, Exception exception, object request);
        void LogInformation(string message);
        void LogInformation(string message, Exception exception, object request);
    }
}