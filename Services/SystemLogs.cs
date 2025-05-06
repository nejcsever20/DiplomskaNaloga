using Microsoft.Extensions.Logging;

namespace diplomska.Services
{
    public interface ISystemLogs
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
    }

    public class SystemLogs : ISystemLogs
    {
        private readonly ILogger<SystemLogs> _logger;

        public SystemLogs(ILogger<SystemLogs> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                _logger.LogError(ex, message);
            }
            else
            {
                _logger.LogError(message);
            }
        }
    }
}

