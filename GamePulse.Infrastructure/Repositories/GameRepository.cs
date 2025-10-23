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
    public class GameRepository : IGameRepository
    {
        public GameRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;

        public async Task AddGamesAsync(List<Game> incomeGames)
        {
            var steamIds = incomeGames.Select(g => g.SteamAppGameId).ToList();

            var existingGames = await _context.Games
                .Where(g => steamIds.Contains(g.SteamAppGameId))
                .ToListAsync();

            var allGenreIds = incomeGames.SelectMany(g => g.Genres).Select(g => g.SteamAppGenreId).Distinct();

            var allTagIds = incomeGames.SelectMany(g => g.Tags).Select(t => t.SteamAppTagId).Distinct();

            var existingGenres = await _context.Genres.Where(g => allGenreIds.Contains(g.SteamAppGenreId)).ToListAsync();

            var existingTags = await _context.Tags.Where(t => allTagIds.Contains(t.SteamAppTagId)).ToListAsync();

            foreach (var game in incomeGames)
            {
                var existingGame = existingGames.FirstOrDefault(g => g.SteamAppGameId == game.SteamAppGameId);

                if (existingGame == null)
                {
                    ProcessGenresAndTags(game, existingGenres, existingTags);
                    _context.Games.Add(game);

                    var gameInfo = new GameInfo
                    {
                        DateOfSearch = DateTime.Now.Date,
                        Game = game,
                        FollowersCount = Random.Shared.Next(0, 1000000)
                    };

                    _context.GameInfos.Add(gameInfo);
                }
                else
                {
                    var gameInfo = new GameInfo
                    {
                        DateOfSearch = DateTime.Now.Date,
                        Game = existingGame,
                        FollowersCount = Random.Shared.Next(0, 1000000)
                    };

                    _context.GameInfos.Add(gameInfo);
                }
            }

            await _context.SaveChangesAsync();
        }

        private void ProcessGenresAndTags(Game game, List<Genre> existingGenres, List<Tag> existingTags)
        {
            for (int i = 0; i < game.Genres.Count; i++)
            {
                var genre = game.Genres[i];
                var existingGenre = existingGenres.FirstOrDefault(g => g.SteamAppGenreId == genre.SteamAppGenreId);
                if (existingGenre != null)
                {
                    game.Genres[i] = existingGenre;
                }
            }

            for (int i = 0; i < game.Tags.Count; i++)
            {
                var tag = game.Tags[i];
                var existingTag = existingTags.FirstOrDefault(t => t.SteamAppTagId == tag.SteamAppTagId);
                if (existingTag != null)
                {
                    game.Tags[i] = existingTag;
                }
            }
        }

        public async Task<List<Game>> GetGamesAsync(int month, Guid? tagId = null, string? platform = null)
        {
            var query = _context.Games
                .Include(g => g.Genres)
                .Include(g => g.Tags)
                .Include(g => g.DatedGameInfo)
                .AsQueryable();

            query = query.Where(g => g.DateOfRelease.Month == month);

            if (tagId.HasValue)
            {
                query = query.Where(g => g.Tags.Any(t => t.Id == tagId));
            }

            if (!string.IsNullOrEmpty(platform))
            {
                query = platform.ToLower() switch
                {
                    "windows" => query.Where(g => g.IsWindowsSupported),
                    "linux" => query.Where(g => g.IsLinuxSupported),
                    "mac" => query.Where(g => g.IsMacSupported),
                    _ => query
                };
            }

            var games = await query.ToListAsync();

            foreach (var game in games)
            {
                var latestInfo = game.DatedGameInfo
                    .OrderByDescending(info => info.DateOfSearch)
                    .FirstOrDefault();

                game.Followers = latestInfo?.FollowersCount ?? 0;
            }

            return games;
        }
    }
}
