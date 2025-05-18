using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetPagedAsync(int pageNumber, int pageSize);
        Task<Project?> GetByIdAsync(int id);
        Task<Project> AddAsync(Project project);
        Task<Project> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int id);
        Task<int> CountAsync();
    }
}
