using GamePulse.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Interfaces.Repositories
{
    public interface IGenreRepository
    {
        public Task<List<Genre>> GetTopGenresWithGamesAsync(int genresCount, int year = 0, int month = 0);
    }
}
