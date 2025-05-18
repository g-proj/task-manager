using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // require valid JWT for all endpoints
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _repo;
        private readonly IMapper _mapper;

        public ProjectsController(IProjectRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET /api/projects?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var total = await _repo.CountAsync();

            var projects = await _repo.GetPagedAsync(pageNumber, pageSize);
            var dtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);

            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(dtos);
        }

        // GET /api/projects/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectDto>> GetById(int id)
        {
            var project = await _repo.GetByIdAsync(id);
            if (project is null) return NotFound();
            return Ok(_mapper.Map<ProjectDto>(project));
        }

        // POST /api/projects
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto)
        {
            var project = _mapper.Map<Project>(dto);
            var created = await _repo.AddAsync(project);
            var result = _mapper.Map<ProjectDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT /api/projects/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProjectDto>> Update(int id, [FromBody] UpdateProjectDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);
            return Ok(_mapper.Map<ProjectDto>(updated));
        }

        // DELETE /api/projects/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repo.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
