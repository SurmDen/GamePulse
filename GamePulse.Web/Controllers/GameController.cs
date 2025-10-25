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

        [Authorize]
        [HttpPost("load")]
        public async Task<IActionResult> LoadGamesAsync()
        {
            try
            {
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
