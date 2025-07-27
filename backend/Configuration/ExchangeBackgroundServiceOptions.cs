namespace SkillForge.Api.Configuration
{
    public class ExchangeBackgroundServiceOptions
    {
        public const string SectionName = "ExchangeBackgroundService";
        
        /// <summary>
        /// Interval in minutes between checks for past exchanges
        /// </summary>
        public int CheckIntervalMinutes { get; set; } = 5;
        
        /// <summary>
        /// Grace period in hours after exchange end time before auto-completing
        /// </summary>
        public double GracePeriodHours { get; set; } = 2;
        
        /// <summary>
        /// Whether the background service is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Maximum number of exchanges to process in a single run
        /// </summary>
        public int BatchSize { get; set; } = 50;
    }
}