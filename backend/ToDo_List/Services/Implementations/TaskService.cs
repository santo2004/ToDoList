using Microsoft.EntityFrameworkCore;
using ToDo_List.Data;
using ToDo_List.DTOs.Task;
using ToDo_List.Enums;
using ToDo_List.Services.Interfaces;

namespace ToDo_List.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskResDto>> GetAllTasks(int userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.Status)
                .ThenBy(t => t.Deadline)
                .ToListAsync();

            return tasks.Select(t => new TaskResDto
            {
                TaskId = t.TaskId,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                Type = t.Type,
                Deadline = t.Deadline,
                CreatedAt = t.CreatedAt
            });
        }

        public async Task<TaskResDto?> GetTaskById(int taskId, int userId)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);

            if (task == null) return null;

            return new TaskResDto
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                Status = task.Status,
                Type = task.Type,
                Deadline = task.Deadline,
                CreatedAt = task.CreatedAt
            };
        }

        public async Task<TaskResDto> CreateTask(int userId, CreateTaskReqDto dto)
        {
            var task = new TaskItem
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = dto.Status,
                Type = dto.Type,
                Deadline = dto.Deadline,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return new TaskResDto
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                Status = task.Status,
                Type = task.Type,
                Deadline = task.Deadline,
                CreatedAt = task.CreatedAt
            };
        }

        public async Task<bool> UpdateTask(int taskId, int userId, UpdateTaskReqDto dto)
        {
            var existing = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);

            if (existing == null) return false;

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Priority = dto.Priority;
            existing.Status = dto.Status;
            existing.Type = dto.Type;
            existing.Deadline = dto.Deadline;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTask(int taskId, int userId)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);

            if (task == null) return false;

            task.IsDeleted = true;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTaskStatus(int taskId, int userId, Enums.TaskStatus status)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.UserId == userId);

            if (task == null) return false;

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;

            if (status == Enums.TaskStatus.Completed)
                task.CompletedAt = DateTime.UtcNow;
            else
                task.CompletedAt = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskResDto>> SearchTasks(int userId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<TaskResDto>();

            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId &&
                    (EF.Functions.Like(t.Title, $"%{query}%") ||
                     EF.Functions.Like(t.Description, $"%{query}%")))
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.Status)
                .ThenBy(t => t.Deadline)
                .ToListAsync();

            return tasks.Select(t => new TaskResDto
            {
                TaskId = t.TaskId,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                Type = t.Type,
                Deadline = t.Deadline,
                CreatedAt = t.CreatedAt
            });
        }
    }
}