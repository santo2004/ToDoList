using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ToDo_List.DTOs.Task;
using ToDo_List.Enums;
using ToDo_List.Responses;
using ToDo_List.Services.Interfaces;

namespace ToDo_List.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasks(GetUserId());

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tasks fetched",
                Data = tasks
            });
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            var task = await _taskService.GetTaskById(taskId, GetUserId());

            if (task == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task fetched",
                Data = task
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskReqDto dto)
        {
            var task = await _taskService.CreateTask(GetUserId(), dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task created",
                Data = task
            });
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(int taskId, [FromBody] UpdateTaskReqDto dto)
        {
            var updated = await _taskService.UpdateTask(taskId, GetUserId(), dto);

            if (!updated)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task updated"
            });
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var deleted = await _taskService.DeleteTask(taskId, GetUserId());

            if (!deleted)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task deleted"
            });
        }

        [HttpPatch("{taskId}/status")]
        public async Task<IActionResult> UpdateStatus(int taskId, [FromQuery] Enums.TaskStatus status)
        {
            var updated = await _taskService.UpdateTaskStatus(taskId, GetUserId(), status);

            if (!updated)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Status updated"
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTasks([FromQuery] string query)
        {
            var tasks = await _taskService.SearchTasks(GetUserId(), query);

            if (tasks == null || !tasks.Any())
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No tasks found"
                });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tasks found",
                Data = tasks
            });
        }
    }
}