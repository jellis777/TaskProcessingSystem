

using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Data;
using TaskProcessing.Api.Enums;
using TaskProcessing.Api.Models;
using TaskProcessing.Worker.Interfaces;

namespace TaskProcessing.Worker.Services
{
    public class TaskProcessor : ITaskProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TaskProcessor> _logger;

        public TaskProcessor(IServiceScopeFactory serviceScopeFactory, ILogger<TaskProcessor> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task ProcessNextQueuedTaskAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var taskItem = await dbContext.Tasks
            .Where(t => t.Status == Status.Queued)
            .OrderBy(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

            if (taskItem is null)
            {
                _logger.LogInformation("No queued tasks found.");
                return;
            }

            await AddLogAsync(dbContext, taskItem.Id, "Task picked up by worker.", "Information", cancellationToken);

            _logger.LogInformation("Picked up task {TaskId} - {Title}", taskItem.Id, taskItem.Title);

            taskItem.Status = Status.Processing;
            taskItem.StartedAt = DateTime.UtcNow;
            taskItem.UpdatedAt = DateTime.UtcNow;

            await AddLogAsync(dbContext, taskItem.Id, "Task marked as Processing.", "Information", cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

                if (taskItem.Title.Contains("fail", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Simulated processing failure.");
                }

                taskItem.Status = Status.Completed;
                taskItem.ResultJson = "{\"message\": \"Task processed successfully.\"}";
                taskItem.ErrorMessage = null;
                taskItem.CompletedAt = DateTime.UtcNow;
                taskItem.UpdatedAt = DateTime.UtcNow;

                await AddLogAsync(dbContext, taskItem.Id, "Task completed successfully.", "Information", cancellationToken);
                _logger.LogInformation("Task {TaskId} completed successfully.", taskItem.Id);
            }
            catch (Exception ex)
            {
                taskItem.ErrorMessage = ex.Message;
                taskItem.ResultJson = null;
                taskItem.CompletedAt = null;
                taskItem.UpdatedAt = DateTime.UtcNow;

                if (taskItem.RetryCount < taskItem.MaxRetries)
                {
                    taskItem.RetryCount += 1;
                    taskItem.Status = Status.Queued;
                    taskItem.StartedAt = null;

                    await AddLogAsync(
                        dbContext,
                        taskItem.Id,
                        $"Task failed: {ex.Message}. Requeued for retry attempt {taskItem.RetryCount} of {taskItem.MaxRetries}.",
                        "Error",
                        cancellationToken);

                    _logger.LogWarning(
                        ex,
                        "Task {TaskId} failed and was requeued for retry attempt {RetryCount} of {MaxRetries}.",
                        taskItem.Id,
                        taskItem.RetryCount,
                        taskItem.MaxRetries);

                }
                else
                {
                    taskItem.Status = Status.Failed;

                    await AddLogAsync(
                        dbContext,
                        taskItem.Id,
                        $"Task failed: {ex.Message}. Max retry limit reached.",
                        "Error",
                        cancellationToken);

                    _logger.LogError(
                        ex,
                        "Task {TaskId} failed and reached the maximum retry limit.",
                        taskItem.Id);

                }

                await AddLogAsync(dbContext, taskItem.Id, $"Task failed: {ex.Message}", "Error", cancellationToken);
                _logger.LogError(ex, "Task {TaskId} failed during processing", taskItem.Id);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static async Task AddLogAsync(
            ApplicationDbContext dbContext,
            int taskItemId,
            string message,
            string level,
            CancellationToken cancellationToken)
        {
            var logEntry = new TaskProcessingLog
            {
                TaskItemId = taskItemId,
                Message = message,
                Level = level,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.TaskProcessingLogs.AddAsync(logEntry, cancellationToken);
        }
    }
}