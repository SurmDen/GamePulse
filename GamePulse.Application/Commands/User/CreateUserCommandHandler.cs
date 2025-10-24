using GamePulse.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Commands.User
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        public CreateUserCommandHandler(IUserRepository userRepository, ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        private readonly IUserRepository _userRepository;
        private ILogger<CreateUserCommandHandler> _logger;

        public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.CreateUserAsync(request.Name, request.Email, request.Passwprd);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Create user error");

                throw;
            }
        }
    }
}
