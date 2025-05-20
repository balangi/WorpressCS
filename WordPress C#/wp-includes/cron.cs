public class CronBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CronBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var cronService = scope.ServiceProvider.GetRequiredService<ICronService>();

            await cronService.RunScheduledJobsAsync();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // every minute
        }
    }
}