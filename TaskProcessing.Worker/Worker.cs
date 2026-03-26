using TaskProcessing.Worker.Interfaces;

namespace TaskProcessing.Worker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceScopeFactory serviceScopeFactory, ILogger<Worker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Task processing worker started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var taskProcessor = scope.ServiceProvider.GetRequiredService<ITaskProcessor>();
                await taskProcessor.ProcessNextQueuedTaskAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while running the worker loop.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Task processing worker stopped at: {time}", DateTimeOffset.Now);
    }
}