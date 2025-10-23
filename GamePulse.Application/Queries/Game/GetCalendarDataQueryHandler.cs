using GamePulse.Application.DTOs.Game;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;
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
        public GetCalendarDataQueryHandler(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        private readonly IGameRepository _gameRepository;

        public async Task<CalendarDto> Handle(GetCalendarDataQuery request, CancellationToken cancellationToken)
        {
            var games = await _gameRepository.GetGamesAsync(request.Month, request.TagId, request.Platform);

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

            return calendar;
        }
    }
}
