public class CronJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Hook { get; set; }
    public DateTime Timestamp { get; set; }
    public string Schedule { get; set; } // daily, hourly, etc.
    public int? Interval { get; set; }   // in seconds
    public Dictionary<string, object> Args { get; set; } = new();
}

public class CronSchedule
{
    public string Name { get; set; }
    public int IntervalInSeconds { get; set; }
    public string Display { get; set; }
}