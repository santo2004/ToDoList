using ToDo_List.Enums;

namespace ToDo_List.DTOs.Task
{
    public class CreateTaskReqDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public TaskPriority Priority { get; set; }
        public ToDo_List.Enums.TaskStatus Status { get; set; }
        public TaskType? Type { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
