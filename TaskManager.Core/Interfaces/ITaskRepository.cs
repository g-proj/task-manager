using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetByProjectAsync(int projectId, int pageNumber, int pageSize);
        Task<TaskItem?> GetByIdAsync(int projectId, int taskId);
        Task<TaskItem> AddAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(int projectId, int taskId);
        Task<int> CountByProjectAsync(int projectId);
    }
}
