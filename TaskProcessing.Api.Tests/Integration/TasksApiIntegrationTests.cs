using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using TaskProcessing.Api.Data;
using TaskProcessing.Api.DTOs;
using TaskProcessing.Api.Enums;
using TaskProcessing.Api.Models;

namespace TaskProcessing.Api.Tests.Integration;

public class TasksApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly CustomWebApplicationFactory _factory;

    public TasksApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.TaskProcessingLogs.RemoveRange(dbContext.TaskProcessingLogs);
        dbContext.Tasks.RemoveRange(dbContext.Tasks);

        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task PostTasks_CreatesTask_AndReturnsCreatedResponse()
    {
        await ResetDatabaseAsync();
        var client = _factory.CreateClient();

        var request = new CreateTaskRequestDto
        {
            Title = "Generate monthly report",
            Description = "Create the March report",
            Type = "report-generation",
            PayloadJson = "{\"month\":\"March\"}"
        };

        var response = await client.PostAsJsonAsync("/api/tasks", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskDetailsDto>(JsonOptions);

        Assert.NotNull(createdTask);
        Assert.Equal("Generate monthly report", createdTask!.Title);
        Assert.Equal(Status.Queued, createdTask.Status);
    }

    [Fact]
    public async Task PostTasks_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        await ResetDatabaseAsync();

        var client = _factory.CreateClient();

        var request = new CreateTaskRequestDto
        {
            Title = "",
            Description = "Invalid request",
            Type = "report-generation",
            PayloadJson = "{\"month\":\"March\"}"
        };

        var response = await client.PostAsJsonAsync("/api/tasks", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_ReturnsSeededTasks()
    {
        await ResetDatabaseAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Tasks.Add(new TaskItem
            {
                Title = "Task one",
                Description = "First task",
                Type = "report-generation",
                Status = Status.Queued
            });

            dbContext.Tasks.Add(new TaskItem
            {
                Title = "Task two",
                Description = "Second task",
                Type = "text-summary",
                Status = Status.Completed
            });

            await dbContext.SaveChangesAsync();
        }

        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskSummaryDto>>(JsonOptions);

        Assert.NotNull(tasks);
        Assert.True(tasks!.Count >= 2);
        Assert.Contains(tasks, t => t.Title == "Task one");
        Assert.Contains(tasks, t => t.Title == "Task two");
    }

    [Fact]
    public async Task GetTaskById_ReturnsTask_WhenTaskExists()
    {
        await ResetDatabaseAsync();

        int taskId;

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var task = new TaskItem
            {
                Title = "Detailed task",
                Description = "Task for details endpoint",
                Type = "file-processing",
                Status = Status.Completed
            };

            dbContext.Tasks.Add(task);
            await dbContext.SaveChangesAsync();

            taskId = task.Id;
        }

        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/tasks/{taskId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var taskDetails = await response.Content.ReadFromJsonAsync<TaskDetailsDto>(JsonOptions);

        Assert.NotNull(taskDetails);
        Assert.Equal(taskId, taskDetails!.Id);
        Assert.Equal("Detailed task", taskDetails.Title);
    }

    [Fact]
    public async Task PostRetry_RequeuesFailedTask()
    {
        await ResetDatabaseAsync();

        int taskId;

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var failedTask = new TaskItem
            {
                Title = "Failed task",
                Description = "Should be retried",
                Type = "report-generation",
                Status = Status.Failed,
                ErrorMessage = "Original error",
                RetryCount = 3,
                MaxRetries = 3
            };

            dbContext.Tasks.Add(failedTask);
            await dbContext.SaveChangesAsync();

            taskId = failedTask.Id;
        }

        var client = _factory.CreateClient();

        var response = await client.PostAsync($"/api/tasks/{taskId}/retry", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var retriedTask = await response.Content.ReadFromJsonAsync<TaskDetailsDto>(JsonOptions);

        Assert.NotNull(retriedTask);
        Assert.Equal(Status.Queued, retriedTask!.Status);
        Assert.Null(retriedTask.ErrorMessage);
        Assert.Equal(4, retriedTask.RetryCount);
    }
}