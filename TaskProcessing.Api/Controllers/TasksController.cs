
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Data;
using TaskProcessing.Api.DTOs;
using TaskProcessing.Api.Enums;
using TaskProcessing.Api.Interfaces;
using TaskProcessing.Api.Models;

namespace TaskProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<ActionResult<TaskDetailsDto>> CreateTask(CreateTaskRequestDto request)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(request);
                return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskSummaryDto>>> GetTasks()
        {
            var tasks = await _taskService.GetTasksAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDetailsDto>> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task is null)
            {
                return NotFound();
            }

            return Ok(task);
        }


        [HttpPost("{id}/retry")]
        public async Task<ActionResult<TaskDetailsDto>> RetryTask(int id)
        {
            var (task, errorMessage) = await _taskService.RetryTaskAsync(id);

            if (task is not null)
            {
                return Ok(task);
            }

            if (errorMessage == "task not found")
            {
                return NotFound();
            }


            return BadRequest(errorMessage);
        }




    }
}