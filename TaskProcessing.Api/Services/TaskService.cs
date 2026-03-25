
using TaskProcessing.Api.Data;
using TaskProcessing.Api.DTOs;
using TaskProcessing.Api.Interfaces;
using TaskProcessing.Api.Models;
using TaskProcessing.Api.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace TaskProcessing.Api.Services
{
    public class TaskService : ITaskService
    {
        private static readonly HashSet<string> AllowedTaskTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "report-generation",
            "text-summary",
            "file-processing"
        };
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TaskDetailsDto> CreateTaskAsync(CreateTaskRequestDto request)
        {
            if (!AllowedTaskTypes.Contains(request.Type))
            {
                throw new ArgumentException(
                    $"Unsupported task type. Allowed types are: {string.Join(", ", AllowedTaskTypes)}"
                );
            }

            if (!string.IsNullOrWhiteSpace(request.PayloadJson) && !IsValidJson(request.PayloadJson))
            {
                throw new ArgumentException("PayloadJson must be valid JSON.");
            }
            var taskItem = new TaskItem
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Type = request.Type.Trim(),
                PayloadJson = request.PayloadJson
            };

            _context.Tasks.Add(taskItem);
            await _context.SaveChangesAsync();

            return MapToTaskDetailsDto(taskItem);
        }


        public async Task<List<TaskSummaryDto>> GetTasksAsync()
        {
            return await _context.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                Type = t.Type,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
        }

        public async Task<TaskDetailsDto?> GetTaskByIdAsync(int id)
        {
            var taskItem = await _context.Tasks
                .Include(t => t.ProcessingLogs.OrderBy(log => log.CreatedAt))
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taskItem is null)
            {
                return null;
            }
            return MapToTaskDetailsDto(taskItem);

        }

        public async Task<(TaskDetailsDto? Task, string? ErrorMessage)> RetryTaskAsync(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);

            if (taskItem is null)
            {
                return (null, "Task not found.");
            }

            if (taskItem.Status != Status.Failed)
            {
                return (null, "Only failed tasks can be retried.");
            }

            taskItem.Status = Status.Queued;
            taskItem.ErrorMessage = null;
            taskItem.ResultJson = null;
            taskItem.StartedAt = null;
            taskItem.CompletedAt = null;
            taskItem.RetryCount += 1;
            taskItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (MapToTaskDetailsDto(taskItem), null);
        }

        private static TaskDetailsDto MapToTaskDetailsDto(TaskItem taskItem)
        {
            return new TaskDetailsDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Type = taskItem.Type,
                Status = taskItem.Status,
                PayloadJson = taskItem.PayloadJson,
                ResultJson = taskItem.ResultJson,
                ErrorMessage = taskItem.ErrorMessage,
                RetryCount = taskItem.RetryCount,
                MaxRetries = taskItem.MaxRetries,
                CreatedAt = taskItem.CreatedAt,
                UpdatedAt = taskItem.UpdatedAt,
                StartedAt = taskItem.StartedAt,
                CompletedAt = taskItem.CompletedAt,
                ProcessingLogs = taskItem.ProcessingLogs
                    .OrderBy(log => log.CreatedAt)
                    .Select(log => new TaskProcessingLogDto
                    {
                        Id = log.Id,
                        Message = log.Message,
                        Level = log.Level,
                        CreatedAt = log.CreatedAt
                    })
                    .ToList()
            };
        }

        private static bool IsValidJson(string json)
        {
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}