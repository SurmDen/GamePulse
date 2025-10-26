using GamePulse.Application.DTOs.User;
using MediatR;

namespace GamePulse.Application.Queries.User
{
    public class GetUserQuery : IRequest<UserDto>
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
