namespace TaskManager.Api.Models;

using DomainTaskStatus = TaskManager.Core.Entities.TaskStatus;

public class UpdateTaskDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DomainTaskStatus Status { get; set; }
}
