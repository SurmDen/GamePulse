using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces.Repositories;
using GamePulse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Infrastructure.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;

        public async Task<List<Genre>> GetTopGenresWithGamesAsync(int genresCount)
        {
            var genres = _context.Genres
                .Include(g => g.Games)
                .AsNoTracking();

            genres = genres.OrderByDescending(g => g.Games.Count()).Take(genresCount);

            foreach (var genre in genres)
            {
                foreach (var game in genre.Games)
                {
                    game.Genres = null;
                }
            }

            return await genres.ToListAsync();
        }
    }
}
