namespace TaskManager.Core.Entities
{
    public enum TaskStatus
    {
        Todo,
        InProgress,
        Done
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public TaskStatus Status { get; set; }

        // Foreign-key back to Project
        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
    }
}
