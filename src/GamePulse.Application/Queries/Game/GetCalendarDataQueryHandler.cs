using GamePulse.Application.DTOs.Game;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Queries.Game
{
    public class GetCalendarDataQueryHandler : IRequestHandler<GetCalendarDataQuery, CalendarDto>
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GetCalendarDataQueryHandler> _logger;

        public GetCalendarDataQueryHandler(IGameRepository gameRepository, ILogger<GetCalendarDataQueryHandler> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<CalendarDto> Handle(GetCalendarDataQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting calendar data for month {Month}, tag: {TagId}, platform: {Platform}",
                request.Month, request.TagId, request.Platform);

            try
            {
                var games = await _gameRepository.GetGamesAsync(request.Month, request.TagId, request.Platform);

                _logger.LogDebug("Retrieved {GameCount} games from repository", games.Count);

                if (!games.Any())
                {
                    _logger.LogWarning("No games found for the specified criteria");
                    return new CalendarDto()
                    {
                        Month = DateTime.Now,
                        Days = new List<DayDto>()
                    };
                }

                var gamesGroupedInfo = games
                    .GroupBy(x => x.DateOfRelease)
                    .ToDictionary(g => g.Key, g => g.ToList());

                CalendarDto calendar = new CalendarDto()
                {
                    Month = games.First().DateOfRelease,
                    Days = gamesGroupedInfo.Select(g => new DayDto()
                    {
                        Day = g.Key,
                        ReleasesCount = g.Value.Count
                    }).ToList()
                };

                _logger.LogInformation("Successfully created calendar with {DayCount} days having releases", calendar.Days.Count);

                return calendar;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar data");
                throw;
            }
        }
    }
}
