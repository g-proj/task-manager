using DomainTaskStatus = TaskManager.Core.Entities.TaskStatus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Infrastructure.Data;
using TaskManager.Core.Entities;

namespace TaskManager.Tests.Integration
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        private readonly bool _asAdmin;
        public CustomWebApplicationFactory()
        {
            _asAdmin = true;
        }
        internal CustomWebApplicationFactory(bool asAdmin)
        {
            _asAdmin = asAdmin;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var dbName = Guid.NewGuid().ToString();

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
                    options.UseInMemoryDatabase(dbName);
                });

                // 3) Override defaults so our “Test” scheme is used automatically
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options =>
                        {
                            options.ClaimsIssuer = _asAdmin ? "Admin" : "User";
                        });
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                });

                // 4) Seed some data
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

                    db.Projects.RemoveRange(db.Projects);
                    db.Tasks.RemoveRange(db.Tasks);
                    db.SaveChanges();

                    var project = new Project { Name = "Seed", Description = "Seed" };
                    db.Projects.Add(project);
                    db.SaveChanges();

                    db.Tasks.AddRange(
                        new TaskItem { Title = "T1", Description = "First seeded task", Status = DomainTaskStatus.Todo, ProjectId = project.Id },
                        new TaskItem { Title = "T2", Description = "Second seeded task", Status = DomainTaskStatus.InProgress, ProjectId = project.Id }
                    );
                    db.SaveChanges();
                }
            });
        }
    }
}
