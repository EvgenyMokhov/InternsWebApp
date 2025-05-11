using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using RabbitMQ;
using HttpDtos;

namespace APIGatewayInterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ServiceManager ServiceManager;
        public ProjectsController(RabbitManager rabbit) => ServiceManager = new(rabbit);

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProjects()
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Projects.GetProjectsAsync(transactionId));
        }

        [HttpPost("get_projects_on_page")]
        public async Task<IActionResult> GetProjectsForPage([FromBody] GetPagedProjectsHttpRequestDto requestData)
        {
            Guid transactionId = Guid.NewGuid();
            if (requestData.PageNumber < 1)
                return BadRequest("Page number cannot be less than 1");
            if (requestData.ProjectCountOnPage < 1)
                return BadRequest("Count on page cannot be less than 1");
            return Ok(await ServiceManager.Projects.GetPagedProjectsAsync(requestData, transactionId));
        }

        [HttpGet("low_detail_all")]
        public async Task<IActionResult> GetLowDetailProjects()
        {
            Guid transactionid = Guid.NewGuid();
            return Ok(await ServiceManager.Projects.GetLowDetailProjectsAsync(transactionid));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] Guid id)
        {
            Guid transactionId = Guid.NewGuid();
            try
            {
                return Ok(await ServiceManager.Projects.GetProjectAsync(id, transactionId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectLowDetailHttpDto project)
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Projects.CreateProjectAsync(project, transactionId));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectHttpDto project)
        {
            if (!project.IsActive && project.Interns.Count != 0)
                return BadRequest("Cannot delete direction with active interns");
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Projects.UpdateProjectAsync(project, transactionId);
            return Ok();
        }
    }
}
