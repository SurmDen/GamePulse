using GamePulse.Application.DTOs.User;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Queries.User
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        public GetUserQueryHandler(IUserRepository userRepository, ILogger<GetUserQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserQueryHandler> _logger;

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByPasswordAndEmailAsync(request.Email, request.Password);

            if (user == null)
            {
                _logger.LogError($"User with email: {request.Email} was null");

                throw new NullReferenceException($"User with email: {request.Email} was null");
            }

            return new UserDto()
            {
                Name = user.UserName,
                Email = user.UserEmail,
                Id = user.Id
            };
        }
    }
}
