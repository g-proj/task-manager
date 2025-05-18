using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Tests.Repositories
{
    public class ProjectRepositoryTests
    {
        private TaskManagerDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
            return new TaskManagerDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Project()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var repo = new ProjectRepository(context);
            var project = new Project { Name = "Test", Description = "Desc" };

            // Act
            var added = await repo.AddAsync(project);
            var all = await context.Projects.ToListAsync();

            // Assert
            Assert.Single(all);
            Assert.Equal("Test", added.Name);
        }

        [Fact]
        public async Task GetPagedAsync_Should_Return_Correct_Page()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var repo = new ProjectRepository(context);
            // seed 5 projects
            for (int i = 1; i <= 5; i++)
                context.Projects.Add(new Project { Name = $"P{i}", Description = $"D{i}" });
            await context.SaveChangesAsync();

            // Act
            var page1 = await repo.GetPagedAsync(pageNumber: 1, pageSize: 2);
            var page2 = await repo.GetPagedAsync(pageNumber: 2, pageSize: 2);

            // Assert
            Assert.Equal(2, page1.Count());
            Assert.Equal("P1", page1.First().Name);
            Assert.Equal(2, page2.Count());
            Assert.Equal("P3", page2.First().Name);
        }

        // TODO: Add tests for GetByIdAsync, UpdateAsync, DeleteAsync...
    }
}
