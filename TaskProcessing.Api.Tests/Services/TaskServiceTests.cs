

using TaskProcessing.Api.Enums;
using TaskProcessing.Api.Models;
using TaskProcessing.Api.Services;
using TaskProcessing.Api.Tests.Helpers;

namespace TaskProcessing.Api.Tests.Services
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task RetryTaskAsync_ReturnsError_WhenTaskDoesNotExist()
        {
            using var context = TestDbContextFactory.Create();
            var service = new TaskService(context);

            var (task, errorMessage) = await service.RetryTaskAsync(999);

            Assert.Null(task);
            Assert.Equal("Task not found.", errorMessage);
        }

        [Fact]
        public async Task RetryTaskAsync_ReturnsError_WhenTaskIsNotFailed()
        {
            using var context = TestDbContextFactory.Create();

            var taskItem = new TaskItem
            {
                Title = "Queued task",
                Description = "Should not retry",
                Type = "report-generation",
                Status = Status.Queued
            };

            context.Tasks.Add(taskItem);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var (task, errorMessage) = await service.RetryTaskAsync(taskItem.Id);

            Assert.Null(task);
            Assert.Equal("Only failed tasks can be retried.", errorMessage);
        }

        [Fact]
        public async Task RetryTaskAsync_RequeuesFailedTask_AndClearsProcessingFields()
        {
            using var context = TestDbContextFactory.Create();

            var failedTask = new TaskItem
            {
                Title = "Failed task",
                Description = "Retry this task",
                Type = "report-generation",
                Status = Status.Failed,
                ErrorMessage = "Something broke",
                ResultJson = "{\"old\":\"result\"}",
                RetryCount = 2,
                MaxRetries = 3,
                StartedAt = DateTime.UtcNow.AddMinutes(-5),
                CompletedAt = DateTime.UtcNow.AddMinutes(-1),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-1)
            };

            context.Tasks.Add(failedTask);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var (task, errorMessage) = await service.RetryTaskAsync(failedTask.Id);

            Assert.Null(errorMessage);
            Assert.NotNull(task);
            Assert.Equal(Status.Queued, task!.Status);
            Assert.Null(task.ErrorMessage);
            Assert.Null(task.ResultJson);
            Assert.Null(task.StartedAt);
            Assert.Null(task.CompletedAt);
            Assert.Equal(3, task.RetryCount);
        }


    }
}