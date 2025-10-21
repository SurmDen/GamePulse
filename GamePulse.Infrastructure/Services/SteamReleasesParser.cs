using GamePulse.Core.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace GamePulse.Infrastructure.Services
{
    public class SteamReleasesParser : IReleasesParser
    {
        public SteamReleasesParser(HttpClient httpClient, ILogger<SteamReleasesParser> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private readonly HttpClient _httpClient;
        private readonly ILogger<SteamReleasesParser> _logger;

        public async Task<List<long>> GetReleaseGamesIdsAsync(int month)
        {
            _logger.LogInformation("Starting GetReleaseGamesIdsAsync for month {Month}", month);
            var gameIds = new List<long>();
            int page = 1;
            bool foundTargetMonth = false;

            while (true)
            {
                _logger.LogDebug("Processing page {Page} for month {Month}", page, month);

                try
                {
                    var url = $"https://store.steampowered.com/search?filter=comingsoon&page={page}&l=english";
                    _logger.LogDebug("Fetching URL: {Url}", url);

                    var html = await _httpClient.GetStringAsync(url);
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var resultsContainer = doc.DocumentNode.SelectSingleNode("//div[@id='search_resultsRows']");

                    if (resultsContainer == null)
                    {
                        _logger.LogInformation("No results container found on page {Page}, stopping", page);
                        break;
                    }

                    var gameLinks = resultsContainer.SelectNodes(".//a[@data-ds-appid]") ?? new HtmlNodeCollection(null);
                    _logger.LogDebug("Found {GameCount} game links on page {Page}", gameLinks.Count, page);

                    if (gameLinks.Count == 0)
                    {
                        _logger.LogInformation("No game links found on page {Page}, stopping", page);
                        break;
                    }

                    int monthGamesOnPage = 0;
                    bool foundLaterMonth = false;

                    foreach (var gameLink in gameLinks)
                    {
                        var releaseDateNode = gameLink.SelectSingleNode(".//div[contains(@class, 'search_released')]");
                        var releaseDateText = releaseDateNode?.InnerText.Trim();

                        _logger.LogDebug("Game link found with release date: {ReleaseDate}", releaseDateText);

                        if (IsTargetMonth(releaseDateText, month))
                        {
                            foundTargetMonth = true;
                            monthGamesOnPage++;

                            var appIdStr = gameLink.GetAttributeValue("data-ds-appid", "");
                            _logger.LogDebug("Target month game found. AppId: {AppId}, Release: {ReleaseDate}", appIdStr, releaseDateText);

                            if (long.TryParse(appIdStr, out long appId))
                            {
                                gameIds.Add(appId);
                                _logger.LogDebug("Added game ID: {GameId} to results", appId);
                            }
                            else
                            {
                                _logger.LogWarning("Failed to parse appId: {AppIdString}", appIdStr);
                            }
                        }
                        else if (foundTargetMonth && IsLaterMonth(releaseDateText, month))
                        {
                            _logger.LogInformation("Found later month game ({ReleaseDate}) after target month, stopping page processing", releaseDateText);
                            foundLaterMonth = true;
                            break;
                        }
                    }

                    _logger.LogInformation("Page {Page} processed: found {MonthGames} target month games, total so far: {TotalGames}",
                        page, monthGamesOnPage, gameIds.Count);

                    if (foundLaterMonth)
                    {
                        _logger.LogInformation("Stopping pagination due to finding later month games");
                        break;
                    }

                    page++;
                    _logger.LogDebug("Waiting before next page request...");
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing page {Page} for month {Month}", page, month);
                    break;
                }
            }

            _logger.LogInformation("Completed GetReleaseGamesIdsAsync for month {Month}. Total games found: {TotalGames}",
                month, gameIds.Count);

            return gameIds;
        }

        private bool IsTargetMonth(string releaseDateText, int targetMonth)
        {
            if (string.IsNullOrEmpty(releaseDateText)) return false;

            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                           "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            var targetMonthName = monthNames[targetMonth - 1];
            return releaseDateText.Contains(targetMonthName) && releaseDateText.Contains("2025");
        }

        private bool IsLaterMonth(string releaseDateText, int targetMonth)
        {
            if (string.IsNullOrEmpty(releaseDateText)) return false;

            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                           "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            for (int laterMonth = targetMonth + 1; laterMonth <= 12; laterMonth++)
            {
                var laterMonthName = monthNames[laterMonth - 1];
                if (releaseDateText.Contains(laterMonthName) && releaseDateText.Contains("2025"))
                    return true;
            }

            return false;
        }
    }
}
