using Microsoft.Extensions.Logging;

namespace Hearth.ArcGIS.Samples.Services
{
    public class TestLogService : ITransientService
    {
        private readonly ILogger<TestLogService> _logger;
        public TestLogService(ILogger<TestLogService> logger)
        {
            _logger = logger;
        }

        public void WriteLog()
        {
            _logger?.LogTrace("Configured Type Logger Class LogTrace");
            _logger?.LogDebug("Configured Type Logger Class LogDebug");
            _logger?.LogInformation("Configured Type Logger Class LogInformation");
            _logger?.LogWarning("Configured Type Logger Class LogWarning");
            _logger?.LogError("Configured Type Logger Class LogError");
            _logger?.LogCritical("Configured Type Logger Class LogCritical");
        }
    }
}