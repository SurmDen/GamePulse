using GamePulse.Application.Commands.User;
using GamePulse.Application.DTOs.User;
using GamePulse.Application.Queries.User;
using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamePulse.Web.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class UserController : ControllerBase
    {
        public UserController(ILogger<UserController> logger, ITokenService tokenService, IMediator mediator)
        {
            _logger = logger;
            _tokenService = tokenService;   
            _mediator = mediator;
        }

        private readonly ILogger<UserController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IMediator _mediator;

        [HttpPost("register")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand userCommand)
        {
            try
            {
                await _mediator.Send(userCommand);

                return Ok( new {message = "Your profile successfully created", code = 200});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create user error");

                return Problem(statusCode: 400, title: "Authorization error", detail: "Please use correct data");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogInAsync([FromBody] GetUserQuery getUser)
        {
            try
            {
                UserDto userDto = await _mediator.Send(getUser);

                User user = new User()
                {
                    UserEmail = userDto.Email,
                    UserName = userDto.Name,
                    Id = userDto.Id
                };

                string token = _tokenService.GetToken(user);

                HttpContext.Session.SetString("token", token);

                return Ok( new {token = token, code = 200});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login user error");

                return Problem(statusCode: 400, title: "Authoentication error", detail: "Please use correct data");
            }
        }

        [Authorize(Policy = "Bearer")]
        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("token");

            return Ok(new { message = "success logout", code = 200 });
        }
    }
}
