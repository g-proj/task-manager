using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskManagerDbContext _context;
        public ProjectRepository(TaskManagerDbContext context)
            => _context = context;

        public async Task<IEnumerable<Project>> GetPagedAsync(int pageNumber, int pageSize)
            => await _context.Projects
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<Project?> GetByIdAsync(int id)
            => await _context.Projects
                .Include(p => p.Tasks)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Project> AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Projects.FindAsync(id);
            if (existing == null) return false;
            _context.Projects.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountAsync()
            => await _context.Projects.CountAsync();
    }
}
