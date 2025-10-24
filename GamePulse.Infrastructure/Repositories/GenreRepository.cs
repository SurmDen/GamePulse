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

        public async Task<List<Genre>> GetTopGenresWithGamesAsync(int genresCount, int year = 0, int month = 0)
        {
            var query = _context.Genres
                .Include(g => g.Games)
                    .ThenInclude(g => g.DatedGameInfo)
                .AsNoTracking();

            if (year > 0 && month > 0)
            {
                query = query.Select(g => new Genre
                {
                    Id = g.Id,
                    GenreName = g.GenreName,
                    SteamAppGenreId = g.SteamAppGenreId,
                    Games = g.Games.Where(game => game.DateOfRelease.Year == year && game.DateOfRelease.Month == month).ToList()
                });
            }

            var genres = await query
                .OrderByDescending(g => g.Games.Count)
                .Take(genresCount)
                .ToListAsync();

            return genres;
        }
    }
}
