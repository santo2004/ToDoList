using System.ComponentModel.DataAnnotations;
using ToDo_List.Enums;

namespace ToDo_List.DTOs.Task
{
    public class CreateTaskReqDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public Enums.TaskStatus Status { get; set; }

        public TaskType? Type { get; set; }

        public DateTime? Deadline { get; set; }
    }
}