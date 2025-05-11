using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using RabbitMQ;
using HttpDtos;

namespace APIGatewayInterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternsController : ControllerBase
    {
        private readonly ServiceManager ServiceManager;
        public InternsController(RabbitManager rabbit) => ServiceManager = new(rabbit);

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInterns()
        {
            Guid transactionId = Guid.NewGuid();
            try
            {
                return Ok(await ServiceManager.Interns.GetInternsAsync(transactionId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("low_detail_all")]
        public async Task<IActionResult> GetLowDetailInterns()
        {
            Guid transactionId = Guid.NewGuid();
            try
            {
                return Ok(await ServiceManager.Interns.GetLowDetailInternsAsync(transactionId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get_filtered")]
        public async Task<IActionResult> GetFilteredInterns([FromQuery] Guid projectId, [FromQuery] Guid directionId)
        {
            Guid transactionId = Guid.NewGuid();
            try
            {
                return Ok(await ServiceManager.Interns.GetFilteredInternsAsync(projectId, directionId, transactionId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIntern([FromRoute] Guid id)
        {
            Guid transactionId = Guid.NewGuid();
            try
            {
                return Ok(await ServiceManager.Interns.GetInternAsync(id, transactionId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateIntern([FromBody] InternLowDetailHttpDto internHttpDto)
        {
            if (!Regex.IsMatch(internHttpDto.User.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Incorrect email");
            if (!Regex.IsMatch(internHttpDto.User.PhoneNumber, @"^\+7\d{10}$") && internHttpDto.User.PhoneNumber != "")
                return BadRequest("Incorrect phone number");
            internHttpDto.Id = Guid.NewGuid();
            Guid transactionId = Guid.NewGuid();
            try
            {
                await ServiceManager.Interns.CreateInternAsync(internHttpDto, transactionId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateIntern([FromBody] InternLowDetailHttpDto internHttpDto)
        {
            if (!Regex.IsMatch(internHttpDto.User.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Incorrect email");
            if (!Regex.IsMatch(internHttpDto.User.PhoneNumber, @"^\+7\d{10}$") && internHttpDto.User.PhoneNumber != "")
                return BadRequest("Incorrect phone number");
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Interns.UpdateInternAsync(internHttpDto, transactionId);
            return Ok();
        }
    }
}
