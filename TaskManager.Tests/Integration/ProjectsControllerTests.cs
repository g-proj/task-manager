using System.Net;
using System.Net.Http.Json;
using TaskManager.Api.Models;
using Xunit;

namespace TaskManager.Tests.Integration
{
    public class ProjectsControllerTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProjectsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_Returns_Seeded_Project_And_Headers()
        {
            // Act
            var response = await _client.GetAsync("/api/projects?pageNumber=1&pageSize=10");

            // Assert status & headers
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.Contains("X-Total-Count"));
            Assert.True(response.Headers.Contains("X-Page-Number"));
            Assert.True(response.Headers.Contains("X-Page-Size"));

            // Assert body
            var projects = await response.Content.ReadFromJsonAsync<ProjectDto[]>();
            Assert.NotNull(projects);
            Assert.Single(projects);
            Assert.Equal("Seed", projects[0].Name);
        }
    }
}
