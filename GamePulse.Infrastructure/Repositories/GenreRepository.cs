using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces.Repositories;
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
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GenreRepository> _logger;

        public GenreRepository(ApplicationDbContext context, ILogger<GenreRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Genre>> GetTopGenresWithGamesAsync(int genresCount, int year = 0, int month = 0)
        {
            _logger.LogInformation("Getting top {GenresCount} genres with games. Year: {Year}, Month: {Month}",
                genresCount, year, month);

            var query = _context.Genres
                .Include(g => g.Games)
                    .ThenInclude(g => g.DatedGameInfo)
                .AsNoTracking();

            if (year > 0 && month > 0)
            {
                _logger.LogDebug("Filtering games by year {Year} and month {Month}", year, month);
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

            _logger.LogInformation("Retrieved {GenreCount} top genres. Top genre has {MaxGameCount} games",
                genres.Count, genres.FirstOrDefault()?.Games.Count ?? 0);

            return genres;
        }
    }
}
