using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces;
using GamePulse.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GamePulse.Infrastructure.Services
{
    public class SteamApiGameParser : IGameParser
    {
        public SteamApiGameParser(IReleasesParser releasesParser, ILogger<SteamApiGameParser> logger, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _releasesParser = releasesParser;
            _logger = logger;
        }

        private readonly IReleasesParser _releasesParser;
        private readonly ILogger<SteamApiGameParser> _logger;
        private readonly HttpClient _httpClient;

        public async Task<List<Game>> GetGamesFromApiAsync(int month)
        {
            _logger.LogInformation("Starting GetGamesFromApiAsync for month {Month}", month);
            List<Game> parsedGames = new List<Game>();

            try
            {
                _logger.LogInformation("Getting game IDs for month {Month}", month);
                List<long> parsedGameIds = await _releasesParser.GetReleaseGamesIdsAsync(month);
                _logger.LogInformation("Retrieved {Count} game IDs for month {Month}", parsedGameIds.Count, month);

                foreach (long id in parsedGameIds)
                {
                    _logger.LogDebug("Processing game ID: {GameId}", id);
                    var response = await _httpClient.GetAsync($"https://store.steampowered.com/api/appdetails?appids={id}&l=english");

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogDebug("Successfully fetched data for game ID: {GameId}", id);
                        string jsonResponseString = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(jsonResponseString))
                        {
                            SteamGameResponse? steamGameResponse = JsonSerializer.Deserialize<SteamGameResponse>(jsonResponseString);

                            if (steamGameResponse != null)
                            {
                                SteamGameData? steamGameData = steamGameResponse?.Data?.Values.FirstOrDefault();

                                if (steamGameData != null)
                                {
                                    _logger.LogDebug("Successfully parsed game data for: {GameName} (ID: {GameId})", steamGameData.Name, id);
                                    Game game = new Game()
                                    {
                                        SteamAppGameId = steamGameData.AppId,
                                        GameName = steamGameData.Name,
                                        DateOfRelease = steamGameData.ReleaseDate.Date,
                                        ShopRef = steamGameData.Website,
                                        ImageRef = steamGameData.HeaderImage,
                                        ShortDescription = steamGameData.ShortDescription,
                                        IsLinuxSupported = steamGameData.Platforms.Linux,
                                        IsMacSupported = steamGameData.Platforms.Mac,
                                        IsWindowsSupported = steamGameData.Platforms.Windows,
                                        GameInfo = new GameInfo()
                                        {
                                            DateOfSearch = DateTime.Now.Date,
                                            Genres = new List<Genre>(steamGameData.Genres.Select(g => new Genre()
                                            {
                                                SteamAppGenreId = long.Parse(g.Id),
                                                GenreName = g.Description
                                            })),
                                            Tags = new List<Tag>(steamGameData.Categories.Select(c => new Tag()
                                            {
                                                SteamAppTagId = c.Id,
                                                TagName = c.Description
                                            }))
                                        }
                                    };

                                    parsedGames.Add(game);
                                    _logger.LogDebug("Added game to list: {GameName}", steamGameData.Name);
                                }
                                else
                                {
                                    _logger.LogWarning("SteamGameData is null for game ID: {GameId}", id);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Failed to deserialize SteamGameResponse for game ID: {GameId}", id);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Empty JSON response for game ID: {GameId}", id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("HTTP request failed for game ID: {GameId}. Status: {StatusCode}", id, response.StatusCode);
                    }
                }

                _logger.LogInformation("Completed processing {TotalGames} games for month {Month}", parsedGames.Count, month);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetGamesFromApiAsync for month {Month}", month);
                throw;
            }

            return parsedGames;
        }
    }
}
