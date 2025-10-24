using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Commands.User
{
    public class CreateUserCommand : IRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Passwprd { get; set; } = string.Empty;
    }
}
