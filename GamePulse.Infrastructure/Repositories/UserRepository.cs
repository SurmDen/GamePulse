using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces.Repositories;
using GamePulse.Core.Interfaces.Services;
using GamePulse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, IPasswordHasher hasher, ILogger<UserRepository> logger)
        {
            _hasher = hasher;
            _context = context;
            _logger = logger;
        }

        public async Task CreateUserAsync(string name, string email, string password)
        {
            _logger.LogInformation("Attempting to create user with email: {Email}", email);

            User? existsUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserEmail == email);

            if (existsUser != null)
            {
                _logger.LogWarning("User creation failed - email already exists: {Email}", email);
                throw new InvalidOperationException($"User with email {email} already exists");
            }

            existsUser = await _context.Users.
                FirstOrDefaultAsync(u => u.PasswordHash == _hasher.GetHash(password));

            if (existsUser != null)
            {
                _logger.LogWarning("User creation failed - password already exists for another user");
                throw new InvalidOperationException("User with entered password already exists");
            }

            await _context.Users.AddAsync(new User()
            {
                UserEmail = email,
                UserName = name,
                PasswordHash = _hasher.GetHash(password)
            });

            await _context.SaveChangesAsync();
            _logger.LogInformation("User successfully created with email: {Email}", email);
        }

        public async Task<User> GetUserByPasswordAndEmailAsync(string email, string password)
        {
            _logger.LogInformation("Attempting to get user by email and password: {Email}", email);

            User? user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserEmail == email && u.PasswordHash == _hasher.GetHash(password));

            if (user == null)
            {
                _logger.LogWarning("User not found with email: {Email}", email);
            }
            else
            {
                _logger.LogInformation("User successfully found with email: {Email}", email);
            }

            return user;
        }
    }
}
