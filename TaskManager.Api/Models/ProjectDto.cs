namespace TaskManager.Api.Models;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
}
