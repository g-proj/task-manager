using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagerDbContext _context;
        public TaskRepository(TaskManagerDbContext context)
            => _context = context;

        public async Task<IEnumerable<TaskItem>> GetByProjectAsync(int projectId, int pageNumber, int pageSize)
            => await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<TaskItem?> GetByIdAsync(int projectId, int taskId)
            => await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId);

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(int projectId, int taskId)
        {
            var existing = await _context.Tasks
                .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId);
            if (existing == null) return false;
            _context.Tasks.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountByProjectAsync(int projectId)
            => await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .CountAsync();
    }
}
