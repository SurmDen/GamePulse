using GamePulse.Application.Events;
using GamePulse.Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamePulse.Web.Controllers
{
    [Route("api/v1/games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        public GameController(ILogger<UserController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;

        //[Authorize(Policy = "Bearer")]
        [HttpPost("load/{month:int}")]
        public async Task<IActionResult> LoadGamesAsync(int month)
        {
            try
            {
                GameSearchEvent gameSearchEvent = new GameSearchEvent()
                {
                    NeededMonth = month,
                    NeededYear = DateTime.Now.Year
                };

                _logger.LogInformation("Event for searching games sended");

                await _mediator.Publish(gameSearchEvent);

                return Ok(new {message = "Event for searching games sended", code = 200 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying to send event");

                return Problem(statusCode:400, title: "Event sending error", detail: "Error occured while trying to send event");
            }
        }
    }
}
