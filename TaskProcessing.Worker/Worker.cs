using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Data;
using TaskProcessing.Api.Enums;

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
                await ProcessQueuedTasksAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing queued tasks.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessQueuedTasksAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var taskItem = await dbContext.Tasks
        .Where(t => t.Status == Status.Queued)
        .OrderBy(t => t.CreatedAt)
        .FirstOrDefaultAsync(stoppingToken);

        if (taskItem is null)
        {
            _logger.LogInformation("No queued tasks found.");
            return;
        }

        _logger.LogInformation("Picked up task {TaskId} - {Title}", taskItem.Id, taskItem.Title);

        taskItem.Status = Status.Processing;
        taskItem.StartedAt = DateTime.UtcNow;
        taskItem.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(stoppingToken);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);

            if (taskItem.Title.Contains("fail", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Simulated processing failure.");
            }

            taskItem.Status = Status.Completed;
            taskItem.ResultJson = "{\"message\": \"Task processed successfully.\"}";
            taskItem.ErrorMessage = null;
            taskItem.CompletedAt = DateTime.UtcNow;
            taskItem.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Task {TaskId} completed successfully.", taskItem.Id);
        }
        catch (Exception ex)
        {
            taskItem.Status = Status.Failed;
            taskItem.ErrorMessage = ex.Message;
            taskItem.CompletedAt = null;
            taskItem.UpdatedAt = DateTime.UtcNow;

            _logger.LogError(ex, "Task {TaskId} failed during processing", taskItem.Id);
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }
}
