using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces.Repositories;
using GamePulse.Core.Interfaces.Services;
using GamePulse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _hasher;

        public UserRepository(ApplicationDbContext context, IPasswordHasher hasher)
        {
            _hasher = hasher;
            _context = context;
        }

        public async Task CreateUserAsync(string name, string email, string password)
        {
            User? existsUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserEmail == email);

            if (existsUser != null)
            {
                throw new InvalidOperationException($"User with email {email} already exists");
            }

            existsUser = await _context.Users.
                FirstOrDefaultAsync(u => u.PasswordHash == _hasher.GetHash(password));

            if (existsUser != null)
            {
                throw new InvalidOperationException("User with entered password already exists");
            }

            await _context.Users.AddAsync(new User()
            {
                UserEmail = email,
                UserName = name,
                PasswordHash = _hasher.GetHash(password)
            });

            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByPasswordAndEmailAsync(string email, string password)
        {
            User? user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserEmail == email && u.PasswordHash == _hasher.GetHash(password));

            return user;
        }
    }
}
