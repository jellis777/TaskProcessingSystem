

using TaskProcessing.Api.DTOs;

namespace TaskProcessing.Api.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDetailsDto> CreateTaskAsync(CreateTaskRequestDto request);
        Task<List<TaskSummaryDto>> GetTasksAsync();
        Task<TaskDetailsDto?> GetTaskByIdAsync(int id);
        Task<(TaskDetailsDto? Task, string? ErrorMessage)> RetryTaskAsync(int id);
    }
}