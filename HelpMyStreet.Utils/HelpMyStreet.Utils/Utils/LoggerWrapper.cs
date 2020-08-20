using Microsoft.Extensions.Logging;
using System;

namespace HelpMyStreet.Utils.Utils
{
    /// <summary>
    /// Wrapper for ILogger to make unit testing easier
    /// </summary>
    public class LoggerWrapper<T> : ILoggerWrapper<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerWrapper(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogError(string message, Exception exception)
        {
            _logger.LogError(exception, message);
        }

        public void LogError(string message, Exception exception, object request)
        {
            _logger.LogError(exception, message, request);
        }

        public void LogErrorAndNotifyNewRelic(string message, Exception exception)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(exception);
            _logger.LogError(exception, message);
        }

        public void LogErrorAndNotifyNewRelic(string message, Exception exception, object request)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(exception);
            _logger.LogError(exception, message, request);
        }

        public void LogWarning(string message, Exception exception)
        {
            _logger.LogWarning(exception, message);
        }

        public void LogWarning(string message, Exception exception, object request)
        {
            _logger.LogWarning(exception, message, request);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogInformation(string message, Exception exception, object request)
        {
            _logger.LogInformation(exception, message, request);
        }
    }
}
