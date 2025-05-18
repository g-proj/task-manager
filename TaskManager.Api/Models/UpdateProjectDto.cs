namespace TaskManager.Api.Models;

public class UpdateProjectDto
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
}
