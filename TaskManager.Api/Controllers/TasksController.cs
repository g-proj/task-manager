using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId:int}/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _repo;
        private readonly IMapper _mapper;

        public TasksController(ITaskRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET /api/projects/{projectId}/tasks?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll(
            int projectId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var total = await _repo.CountByProjectAsync(projectId);
            var tasks = await _repo.GetByProjectAsync(projectId, pageNumber, pageSize);
            var dtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);

            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page-Number"] = pageNumber.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(dtos);
        }

        // GET /api/projects/{projectId}/tasks/{taskId}
        [HttpGet("{taskId:int}")]
        public async Task<ActionResult<TaskDto>> GetById(int projectId, int taskId)
        {
            var task = await _repo.GetByIdAsync(projectId, taskId);
            if (task is null) return NotFound();
            return Ok(_mapper.Map<TaskDto>(task));
        }

        // POST /api/projects/{projectId}/tasks
        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create(
            int projectId,
            [FromBody] CreateTaskDto dto)
        {
            var task = _mapper.Map<TaskItem>(dto);
            task.ProjectId = projectId;
            var created = await _repo.AddAsync(task);
            var result = _mapper.Map<TaskDto>(created);
            return CreatedAtAction(nameof(GetById),
                new { projectId = projectId, taskId = result.Id }, result);
        }

        // PUT /api/projects/{projectId}/tasks/{taskId}
        [HttpPut("{taskId:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<TaskDto>> Update(
            int projectId,
            int taskId,
            [FromBody] UpdateTaskDto dto)
        {
            var existing = await _repo.GetByIdAsync(projectId, taskId);
            if (existing is null) return NotFound();

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);
            return Ok(_mapper.Map<TaskDto>(updated));
        }

        // DELETE /api/projects/{projectId}/tasks/{taskId}
        [HttpDelete("{taskId:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int projectId, int taskId)
        {
            var success = await _repo.DeleteAsync(projectId, taskId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
