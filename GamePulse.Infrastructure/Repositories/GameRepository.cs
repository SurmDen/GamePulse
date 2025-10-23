using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        public async Task AddGamesAsync(List<Game> games)
        {
            
        }

        public async Task<List<Game>> GetGamesAsync(int month, long? tagId = null, string? platform = null)
        {
            
        }
    }
}
