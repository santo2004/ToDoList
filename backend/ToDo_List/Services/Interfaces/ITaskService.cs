using ToDo_List.DTOs.Task;

namespace ToDo_List.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResDto>> GetAllTasks(int userId);
        Task<TaskResDto?> GetTaskById(int taskId, int userId);
        Task<TaskResDto> CreateTask(int userId, CreateTaskReqDto dto);
        Task<bool> UpdateTask(int taskId, int userId, UpdateTaskReqDto dto);
        Task<bool> DeleteTask(int taskId, int userId);
        Task<bool> UpdateTaskStatus(int taskId, int userId, Enums.TaskStatus status);
        Task<IEnumerable<TaskResDto>> SearchTasks(int userId, string query);
    }
}