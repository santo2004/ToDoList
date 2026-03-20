using ToDo_List.Enums;

namespace ToDo_List.Entities
{
    public class TaskItem
    {
        public int TaskId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public TaskPriority Priority { get; set; } 
        public ToDo_List.Enums.TaskStatus Status { get; set; }
        public TaskType? Type { get; set; }
        public DateTime? Deadline { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public bool IsDeleted { get; set; }

        public User User { get; set; } = null!;
    }
}