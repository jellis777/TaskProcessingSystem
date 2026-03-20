
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Data;
using TaskProcessing.Api.DTOs;
using TaskProcessing.Api.Enums;
using TaskProcessing.Api.Models;

namespace TaskProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TaskDetailsDto>> CreateTask(CreateTaskRequestDto request)
        {
            var taskItem = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                PayloadJson = request.PayloadJson
            };

            _context.Tasks.Add(taskItem);
            await _context.SaveChangesAsync();

            var response = MapToTaskDetailsDto(taskItem);

            return CreatedAtAction(nameof(GetTaskById), new { id = taskItem.Id }, response);
        }

        [HttpPost("{id}/retry")]
        public async Task<ActionResult<TaskDetailsDto>> RetryTask(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);

            if (taskItem is null)
            {
                return NotFound();
            }

            if (taskItem.Status != Status.Failed)
            {
                return BadRequest("Only failed tasks can be retried.");
            }

            if (taskItem.RetryCount >= taskItem.MaxRetries)
            {
                return BadRequest("This task has reached its maximum retry limit.");
            }

            taskItem.Status = Status.Queued;
            taskItem.ErrorMessage = null;
            taskItem.ResultJson = null;
            taskItem.StartedAt = null;
            taskItem.CompletedAt = null;
            taskItem.RetryCount += 1;
            taskItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = MapToTaskDetailsDto(taskItem);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskSummaryDto>>> GetTasks()
        {
            var tasks = await _context.Tasks
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
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDetailsDto>> GetTaskById(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);

            if (taskItem is null)
            {
                return NotFound();
            }
            var response = MapToTaskDetailsDto(taskItem);
            return Ok(response);
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
                CompletedAt = taskItem.CompletedAt
            };
        }




    }
}