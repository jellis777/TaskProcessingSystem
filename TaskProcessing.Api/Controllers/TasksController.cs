
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Data;
using TaskProcessing.Api.DTOs;
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

            var response = new TaskDetailsDto
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

            return CreatedAtAction(nameof(GetTaskById), new { id = taskItem.Id }, response);
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
            var response = new TaskDetailsDto
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
            return Ok(response);
        }




    }
}