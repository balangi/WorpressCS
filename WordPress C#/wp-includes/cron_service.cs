public class CronService : ICronService
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public CronService(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public async Task ScheduleSingleEventAsync(string hook, DateTime timestamp, Dictionary<string, object> args = null)
    {
        var job = new CronJob
        {
            Hook = hook,
            Timestamp = timestamp,
            Args = args ?? new Dictionary<string, object>(),
        };

        await _context.CronJobs.AddAsync(job);
        await _context.SaveChangesAsync();
    }

    public async Task ScheduleRecurringEventAsync(string hook, string scheduleName, DateTime startTimestamp, Dictionary<string, object> args = null)
    {
        var schedules = await GetSchedulesAsync();
        var schedule = schedules.FirstOrDefault(s => s.Name == scheduleName);

        if (schedule == null)
            throw new ArgumentException($"Schedule '{scheduleName}' not found.");

        var job = new CronJob
        {
            Hook = hook,
            Timestamp = startTimestamp,
            Schedule = scheduleName,
            Interval = schedule.IntervalInSeconds,
            Args = args ?? new Dictionary<string, object>(),
        };

        await _context.CronJobs.AddAsync(job);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CronJob>> GetReadyJobsAsync()
    {
        return await _context.CronJobs
            .Where(j => j.Timestamp <= DateTime.UtcNow)
            .OrderBy(j => j.Timestamp)
            .ToListAsync();
    }

    public async Task<List<CronSchedule>> GetSchedulesAsync()
    {
        return new List<CronSchedule>
        {
            new() { Name = "hourly", IntervalInSeconds = 3600, Display = "Hourly" },
            new() { Name = "daily", IntervalInSeconds = 86400, Display = "Daily" },
            new() { Name = "weekly", IntervalInSeconds = 604800, Display = "Weekly" },
            new() { Name = "monthly", IntervalInSeconds = 2592000, Display = "Monthly" },
            new() { Name = "twicedaily", IntervalInSeconds = 43200, Display = "Twice Daily" }
        };
    }

    public async Task<bool> RunScheduledJobsAsync()
    {
        var jobs = await GetReadyJobsAsync();
        if (!jobs.Any()) return true;

        foreach (var job in jobs)
        {
            try
            {
                await ExecuteHookAsync(job.Hook, job.Args);
                if (job.Interval.HasValue)
                {
                    job.Timestamp = job.Timestamp.AddSeconds(job.Interval.Value);
                    _context.CronJobs.Update(job);
                }
                else
                {
                    _context.CronJobs.Remove(job);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing cron job '{job.Hook}': {ex.Message}");
                return false;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task ExecuteHookAsync(string hook, Dictionary<string, object> args)
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        switch (hook)
        {
            case "test_cron":
                var logger = services.GetRequiredService<ILogger<CronService>>();
                logger.LogInformation("Test Cron executed at {Time}", DateTime.UtcNow);
                break;

            default:
                Console.WriteLine($"Hook '{hook}' is not registered.");
                break;
        }
    }

    public async Task UnscheduleEventAsync(string hook, DateTime timestamp, Dictionary<string, object> args = null)
    {
        var query = _context.CronJobs.Where(j => j.Hook == hook && j.Timestamp == timestamp);
        if (args != null && args.Count > 0)
        {
            foreach (var arg in args)
            {
                query = query.Where(j => j.Args.ContainsKey(arg.Key) && j.Args[arg.Key].ToString() == arg.Value.ToString());
            }
        }

        var jobs = await query.ToListAsync();
        if (jobs.Count > 0)
        {
            _context.CronJobs.RemoveRange(jobs);
            await _context.SaveChangesAsync();
        }
    }
}