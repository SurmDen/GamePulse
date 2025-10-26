using GamePulse.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Infrastructure.Services
{
    public class SHA256Hasher : IPasswordHasher
    {
        public string GetHash(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
