using DomainTaskStatus = TaskManager.Core.Entities.TaskStatus;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using TaskManager.Api.Models;


namespace TaskManager.Tests.Integration
{
    public class TasksControllerTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private const int ProjectId = 1;

        public TasksControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_Returns_Seeded_Tasks_And_Headers()
        {
            // Act
            var response = await _client.GetAsync(
                $"/api/projects/{ProjectId}/tasks?pageNumber=1&pageSize=10"
            );

            // Assert status & headers
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Total-Count"));
            Assert.True(response.Headers.Contains("X-Page-Number"));
            Assert.True(response.Headers.Contains("X-Page-Size"));

            // Assert body
            var tasks = await response.Content.ReadFromJsonAsync<TaskDto[]>();
            Assert.NotNull(tasks);
            Assert.Equal(2, tasks!.Length);
            Assert.Contains(tasks, t => t.Title == "T1");
            Assert.Contains(tasks, t => t.Title == "T2");
        }

        [Fact]
        public async Task Create_Update_Delete_Task_Works_As_Admin()
        {
            // POST a new task
            var newTask = new CreateTaskDto
            {
                Title = "NewTask",
                Description = "Created in test",
                Status = DomainTaskStatus.Todo
            };
            var postResp = await _client.PostAsJsonAsync(
                $"/api/projects/{ProjectId}/tasks", newTask
            );
            Assert.Equal(HttpStatusCode.Created, postResp.StatusCode);

            var created = await postResp.Content.ReadFromJsonAsync<TaskDto>();
            Assert.NotNull(created);
            Assert.Equal("NewTask", created!.Title);

            // PUT update it
            var update = new UpdateTaskDto
            {
                Title = "Updated",
                Description = "Updated desc",
                Status = DomainTaskStatus.Done
            };
            var putResp = await _client.PutAsJsonAsync(
                $"/api/projects/{ProjectId}/tasks/{created.Id}", update
            );
            Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

            var updated = await putResp.Content.ReadFromJsonAsync<TaskDto>();
            Assert.Equal("Updated", updated!.Title);
            Assert.Equal(DomainTaskStatus.Done, updated.Status);

            // DELETE it
            var delResp = await _client.DeleteAsync(
                $"/api/projects/{ProjectId}/tasks/{created.Id}"
            );
            Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);
        }

        [Fact]
        public async Task Delete_Without_AdminRole_Is_Forbidden()
        {
            var nonAdminFactory = new CustomWebApplicationFactory(false);
            var unauthClient = nonAdminFactory.CreateClient();

            // Attempt delete of seeded task 1
            var resp = await unauthClient.DeleteAsync(
                $"/api/projects/{ProjectId}/tasks/1"
            );
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }
    }
}
