using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SkillForge.Api.HealthChecks
{
    public class ExchangeBackgroundServiceHealthCheck : IHealthCheck
    {
        private static DateTime _lastRunTime = DateTime.UtcNow;
        private static bool _isHealthy = true;
        private static string _lastError = string.Empty;
        
        public static void ReportSuccess()
        {
            _lastRunTime = DateTime.UtcNow;
            _isHealthy = true;
            _lastError = string.Empty;
        }
        
        public static void ReportError(string error)
        {
            _lastRunTime = DateTime.UtcNow;
            _isHealthy = false;
            _lastError = error;
        }
        
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var timeSinceLastRun = DateTime.UtcNow - _lastRunTime;
            
            // Consider unhealthy if hasn't run in over 15 minutes
            if (timeSinceLastRun > TimeSpan.FromMinutes(15))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Background service hasn't run in {timeSinceLastRun.TotalMinutes:F1} minutes"));
            }
            
            if (!_isHealthy)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Background service reported error: {_lastError}"));
            }
            
            return Task.FromResult(HealthCheckResult.Healthy(
                $"Background service last ran {timeSinceLastRun.TotalMinutes:F1} minutes ago"));
        }
    }
}