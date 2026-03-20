using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ToDo_List.DTOs.Task;
using ToDo_List.Enums;
using ToDo_List.Services.Interfaces;

namespace ToDo_List.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tasks = await _taskService.GetAllTasks(userId);
            return Ok(tasks);
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var task = await _taskService.GetTaskById(taskId, userId);

            if (task == null)
                return NotFound("Task not found");

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskReqDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var task = await _taskService.CreateTask(userId, dto);
            return Ok(task);
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(int taskId, [FromBody] UpdateTaskReqDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var updated = await _taskService.UpdateTask(taskId, userId, dto);

            if (!updated)
                return NotFound("Task not found");

            return Ok("Task updated successfully");
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var deleted = await _taskService.DeleteTask(taskId, userId);

            if (!deleted)
                return NotFound("Task not found");

            return Ok("Task deleted successfully");
        }

        [HttpPatch("{taskId}/status")]
        public async Task<IActionResult> UpdateStatus(int taskId,[FromQuery] Enums.TaskStatus status)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var updated = await _taskService.UpdateTaskStatus(taskId, userId, status);

            if (!updated)
                return NotFound("Task not found");

            return Ok("Status updated successfully");
        }
    }
}