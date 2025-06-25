using Microsoft.Extensions.Logging;

namespace Pawfectmatch_V._1.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            if (exception != null)
            {
                _logger.LogError(exception, message);
            }
            else
            {
                _logger.LogError(message);
            }
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }

        public void LogCritical(string message, Exception? exception = null)
        {
            if (exception != null)
            {
                _logger.LogCritical(exception, message);
            }
            else
            {
                _logger.LogCritical(message);
            }
        }

        public void LogTrace(string message)
        {
            _logger.LogTrace(message);
        }
    }
} 