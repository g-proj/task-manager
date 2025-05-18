namespace TaskManager.Api.Models;

using DomainTaskStatus = TaskManager.Core.Entities.TaskStatus;

public class CreateTaskDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DomainTaskStatus Status { get; set; }
}
