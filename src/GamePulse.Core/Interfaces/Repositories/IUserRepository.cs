using GamePulse.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task CreateUserAsync(string name, string email, string password);

        public Task<User> GetUserByPasswordAndEmailAsync(string email, string password);
    }
}
