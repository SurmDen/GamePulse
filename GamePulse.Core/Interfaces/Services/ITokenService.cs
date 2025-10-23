using GamePulse.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Interfaces.Services
{
    public interface ITokenService
    {
        public string GetToken(User user);
    }
}
