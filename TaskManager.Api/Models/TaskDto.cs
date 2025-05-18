namespace TaskManager.Api.Models;

using DomainTaskStatus = TaskManager.Core.Entities.TaskStatus;

public class TaskDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DomainTaskStatus Status { get; set; }
}
