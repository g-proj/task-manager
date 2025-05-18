namespace TaskManager.Api.Models;
public class CreateProjectDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
}
