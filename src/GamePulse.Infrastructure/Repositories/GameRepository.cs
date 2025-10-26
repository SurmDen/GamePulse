using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces.Repositories;
using GamePulse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GamePulse.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GameRepository> _logger;

        public GameRepository(ApplicationDbContext context, ILogger<GameRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddGamesAsync(List<Game> incomeGames)
        {
            _logger.LogInformation("Starting to add {GameCount} games to repository", incomeGames.Count);

            var steamIds = incomeGames.Select(g => g.SteamAppGameId).ToList();

            var existingGames = await _context.Games
                .Where(g => steamIds.Contains(g.SteamAppGameId))
                .ToListAsync();

            _logger.LogDebug("Found {ExistingGameCount} existing games matching Steam IDs", existingGames.Count);

            var allGenreIds = incomeGames.SelectMany(g => g.Genres).Select(g => g.SteamAppGenreId).Distinct();

            var allTagIds = incomeGames.SelectMany(g => g.Tags).Select(t => t.SteamAppTagId).Distinct();

            var existingGenres = await _context.Genres.Where(g => allGenreIds.Contains(g.SteamAppGenreId)).ToListAsync();

            var existingTags = await _context.Tags.Where(t => allTagIds.Contains(t.SteamAppTagId)).ToListAsync();

            _logger.LogDebug("Found {ExistingGenreCount} existing genres and {ExistingTagCount} existing tags", existingGenres.Count, existingTags.Count);

            int newGamesCount = 0;
            int existingGamesCount = 0;

            foreach (var game in incomeGames)
            {
                var existingGame = existingGames.FirstOrDefault(g => g.SteamAppGameId == game.SteamAppGameId);

                if (existingGame == null)
                {
                    _logger.LogDebug("Adding new game: {GameName} (Steam ID: {SteamId})", game.GameName, game.SteamAppGameId);

                    ProcessGenresAndTags(game, existingGenres, existingTags);

                    _context.Games.Add(game);

                    var gameInfo = new GameInfo
                    {
                        DateOfSearch = DateTime.UtcNow.Date,
                        Game = game,
                        FollowersCount = Random.Shared.Next(0, 1000000)
                    };

                    _context.GameInfos.Add(gameInfo);
                    newGamesCount++;
                }
                else
                {
                    _logger.LogDebug("Game already exists, adding info only: {GameName} (Steam ID: {SteamId})",
                        existingGame.GameName, existingGame.SteamAppGameId);
                    var gameInfo = new GameInfo
                    {
                        DateOfSearch = DateTime.UtcNow.Date,
                        Game = existingGame,
                        FollowersCount = Random.Shared.Next(0, 1000000)
                    };

                    _context.GameInfos.Add(gameInfo);
                    existingGamesCount++;
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added games: {NewGamesCount} new, {ExistingGamesCount} existing with updated info", newGamesCount, existingGamesCount);
        }

        private void ProcessGenresAndTags(Game game, List<Genre> existingGenres, List<Tag> existingTags)
        {
            _logger.LogDebug("Processing genres and tags for game: {GameName}", game.GameName);

            int replacedGenres = 0;
            int replacedTags = 0;

            for (int i = 0; i < game.Genres.Count; i++)
            {
                var genre = game.Genres[i];
                var existingGenre = existingGenres.FirstOrDefault(g => g.SteamAppGenreId == genre.SteamAppGenreId);
                if (existingGenre != null)
                {
                    game.Genres[i] = existingGenre;
                    replacedGenres++;
                }
            }

            for (int i = 0; i < game.Tags.Count; i++)
            {
                var tag = game.Tags[i];
                var existingTag = existingTags.FirstOrDefault(t => t.SteamAppTagId == tag.SteamAppTagId);
                if (existingTag != null)
                {
                    game.Tags[i] = existingTag;
                    replacedTags++;
                }
            }

            _logger.LogDebug("Replaced {ReplacedGenres} genres and {ReplacedTags} tags with existing entities", replacedGenres, replacedTags);
        }

        public async Task<List<Game>> GetGamesAsync(int month, Guid? tagId = null, string? platform = null)
        {
            _logger.LogInformation("Getting games for month {Month} with tagId: {TagId}, platform: {Platform}",
                month, tagId, platform);

            var query = _context.Games
                .Include(g => g.Genres)
                .Include(g => g.Tags)
                .Include(g => g.DatedGameInfo)
                .AsQueryable();

            query = query.Where(g => g.DateOfRelease.Month == month);

            if (tagId.HasValue)
            {
                _logger.LogDebug("Filtering by tag ID: {TagId}", tagId);
                query = query.Where(g => g.Tags.Any(t => t.Id == tagId));
            }

            if (!string.IsNullOrEmpty(platform))
            {
                _logger.LogDebug("Filtering by platform: {Platform}", platform);
                query = platform.ToLower() switch
                {
                    "windows" => query.Where(g => g.IsWindowsSupported),
                    "linux" => query.Where(g => g.IsLinuxSupported),
                    "mac" => query.Where(g => g.IsMacSupported),
                    _ => query
                };
            }

            var games = await query.ToListAsync();
            _logger.LogInformation("Retrieved {GameCount} games from database", games.Count);

            foreach (var game in games)
            {
                var latestInfo = game.DatedGameInfo
                    .OrderByDescending(info => info.DateOfSearch)
                    .FirstOrDefault();

                game.Followers = latestInfo?.FollowersCount ?? 0;
            }

            _logger.LogDebug("Updated followers count for all retrieved games");

            return games;
        }
    }
}
