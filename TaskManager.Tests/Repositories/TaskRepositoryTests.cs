using DomainTaskStatus = TaskManager.Core.Entities.TaskStatus;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Tests.Repositories
{
    public class TaskRepositoryTests
    {
        private TaskManagerDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
                .UseInMemoryDatabase($"TestDb_{System.Guid.NewGuid()}")
                .Options;
            return new TaskManagerDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_TaskItem()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var project = new Project { Name = "P", Description = "D" };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            var repo = new TaskRepository(context);
            var task = new TaskItem 
            { 
                Title = "T1", 
                Description = "Desc1", 
                Status = DomainTaskStatus.Todo, 
                ProjectId = project.Id 
            };

            // Act
            var added = await repo.AddAsync(task);
            var all   = await context.Tasks.ToListAsync();

            // Assert
            Assert.Single(all);
            Assert.Equal("T1", added.Title);
        }

        [Fact]
        public async Task GetByProjectAsync_Should_Return_Correct_Page()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var project = new Project { Name = "P", Description = "D" };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            for (int i = 1; i <= 5; i++)
            {
                context.Tasks.Add(new TaskItem 
                { 
                    Title = $"T{i}", 
                    Description = $"D{i}", 
                    Status = DomainTaskStatus.Todo, 
                    ProjectId = project.Id 
                });
            }
            await context.SaveChangesAsync();

            var repo = new TaskRepository(context);

            // Act
            var page1 = await repo.GetByProjectAsync(project.Id, pageNumber: 1, pageSize: 2);
            var page2 = await repo.GetByProjectAsync(project.Id, pageNumber: 2, pageSize: 2);

            // Assert
            Assert.Equal(2, page1.Count());
            Assert.Equal("T1", page1.First().Title);
            Assert.Equal(2, page2.Count());
            Assert.Equal("T3", page2.First().Title);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Task()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var project = new Project { Name = "P", Description = "D" };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            var task = new TaskItem 
            { 
                Title = "T1", 
                Description = "D1", 
                Status = DomainTaskStatus.Todo, 
                ProjectId = project.Id 
            };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            var repo = new TaskRepository(context);

            // Act
            var fetched = await repo.GetByIdAsync(project.Id, task.Id);

            // Assert
            Assert.NotNull(fetched);
            Assert.Equal("T1", fetched!.Title);
        }

        // TODO: Add tests for UpdateAsync and DeleteAsync
    }
}
