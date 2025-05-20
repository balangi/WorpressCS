public interface ICronService
{
    Task ScheduleSingleEventAsync(string hook, DateTime timestamp, Dictionary<string, object> args = null);
    Task ScheduleRecurringEventAsync(string hook, string scheduleName, DateTime startTimestamp, Dictionary<string, object> args = null);
    Task UnscheduleEventAsync(string hook, DateTime timestamp, Dictionary<string, object> args = null);
    Task<List<CronJob>> GetReadyJobsAsync();
    Task<bool> RunScheduledJobsAsync();
    Task<List<CronSchedule>> GetSchedulesAsync();
}