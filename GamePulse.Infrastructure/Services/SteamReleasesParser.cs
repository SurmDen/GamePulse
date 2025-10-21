using GamePulse.Core.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var gameIds = new List<long>();
            int page = 1;

            while (true)
            {
                var url = $"https://store.steampowered.com/search?filter=comingsoon&page={page}&l=english";
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var resultsContainer = doc.DocumentNode.SelectSingleNode("//div[@id='search_resultsRows']");

                if (resultsContainer == null) break;

                var gameLinks = resultsContainer.SelectNodes(".//a[@data-ds-appid]") ?? new HtmlNodeCollection(null);

                if (gameLinks.Count == 0) break;

                var pageIds = new List<long>();

                foreach (var gameLink in gameLinks)
                {
                    var releaseDateNode = gameLink.SelectSingleNode(".//div[contains(@class, 'search_released')]");

                    var releaseDateText = releaseDateNode?.InnerText.Trim();

                    if (IsTargetMonth(releaseDateText, month))
                    {
                        var appIdStr = gameLink.GetAttributeValue("data-ds-appid", "");

                        if (long.TryParse(appIdStr, out long appId))
                        {
                            pageIds.Add(appId);
                        }
                    }
                }

                if (pageIds.Count > 0)
                {
                    gameIds.AddRange(pageIds);
                }

                page++;

                await Task.Delay(100);
            }

            return gameIds;
        }

        private bool IsTargetMonth(string releaseDateText, int targetMonth)
        {
            if (string.IsNullOrEmpty(releaseDateText)) return false;

            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                           "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            var targetMonthName = monthNames[targetMonth - 1];

            return releaseDateText.ToLower().Contains(targetMonthName.ToLower());
        }

        private bool IsFutureMonth(string releaseDateText, int targetMonth)
        {
            if (string.IsNullOrEmpty(releaseDateText)) return false;

            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                           "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            for (int futureMonth = targetMonth + 1; futureMonth <= 12; futureMonth++)
            {
                var futureMonthName = monthNames[futureMonth - 1];
                if (releaseDateText.Contains(futureMonthName) && releaseDateText.Contains("2025"))
                    return true;
            }

            return false;
        }
    }
}
