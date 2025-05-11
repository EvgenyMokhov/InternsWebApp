using BusinessLogic;
using HttpDtos;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ;

namespace APIGatewayInterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectionsController : ControllerBase
    {
        private readonly ServiceManager ServiceManager;
        private readonly ILogger<DirectionsController> Logger;
        public DirectionsController(RabbitManager rabbit, ILogger<DirectionsController> logger) 
        { 
            ServiceManager = new(rabbit);  
            Logger = logger; 
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllDirections()
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Directions.GetDirectionsAsync(transactionId));
        }

        [HttpPost("get_directions_on_page")]
        public async Task<IActionResult> GetDirectionsForPage([FromBody] GetPagedDirectionsHttpRequestDto requestData )
        {
            Guid transactionId = Guid.NewGuid();
            if (requestData.PageNumber < 1)
                return BadRequest("Page number cannot be less than 1");
            if (requestData.DirectionsCountOnPage < 1)
                return BadRequest("Count on page cannot be less than 1");
            return Ok(await ServiceManager.Directions.GetPagedDirectionsAsync(requestData, transactionId));
        }

        [HttpGet("low_detail_all")]
        public async Task<IActionResult> GetLowDetailDirections()
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Directions.GetDirectionsLowDetailAsync(transactionId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDirection([FromRoute] Guid id)
        {
            Guid transactionId = Guid.NewGuid();
            try
            {
                return Ok(await ServiceManager.Directions.GetDirectionAsync(id, transactionId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDirection([FromBody] DirectionLowDetailHttpDto direciton)
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Directions.CreateDirectionAsync(direciton, transactionId));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateDirection([FromBody] DirectionHttpDto direction)
        {
            if (!direction.IsActive && direction.Interns.Count != 0)
                return BadRequest("Cannot delete direction with active interns");
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Directions.UpdateDirectionAsync(direction, transactionId);
            return Ok();
        } 
    }
}
