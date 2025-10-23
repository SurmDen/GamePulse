using GamePulse.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Interfaces.Repositories
{
    public interface IGameRepository
    {
        public Task AddGamesAsync(List<Game> games);

        public Task<List<Game>> GetGamesAsync(int month, long? tagId = null, string? platform = null);
    }
}