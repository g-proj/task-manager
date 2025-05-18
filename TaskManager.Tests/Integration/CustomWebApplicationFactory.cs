using DomainTaskStatus = TaskManager.Core.Entities.TaskStatus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using TaskManager.Core.Entities;

namespace TaskManager.Tests.Integration
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // 1) Remove the real DbContext registration
                var descriptor = services
                    .Single(d => d.ServiceType == typeof(DbContextOptions<TaskManagerDbContext>));
                services.Remove(descriptor);

                // 2) Register DbContext with InMemory for testing
                services.AddDbContext<TaskManagerDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // 3) Override defaults so our “Test” scheme is used automatically
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", options => { }
                );

                services.AddAuthorization(options =>
                    options.AddPolicy("AdminOnly",
                        p => p.RequireClaim("role", "Admin"))
                );

                // 4) Seed some data
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

                // projects
                var project = new Project
                {
                    Name = "Seed",
                    Description = "Seed"
                };
                db.Projects.Add(project);
                db.SaveChanges();

                // Tasks
                db.Tasks.AddRange(
                [
                    new TaskItem
                    {
                        Title       = "T1",
                        Description = "First seeded task",
                        Status      = DomainTaskStatus.Todo,
                        ProjectId   = project.Id
                    },
                    new TaskItem
                    {
                        Title       = "T2",
                        Description = "Second seeded task",
                        Status      = DomainTaskStatus.InProgress,
                        ProjectId   = project.Id
                    }
                ]);
                db.SaveChanges();
            });
        }
    }
}
