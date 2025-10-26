using GamePulse.Application.DTOs.Game;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Queries.Game
{
    public class GetGamesDataQueryHandler : IRequestHandler<GetGamesDataQuery, List<GameDto>>
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GetGamesDataQueryHandler> _logger;

        public GetGamesDataQueryHandler(IGameRepository gameRepository, ILogger<GetGamesDataQueryHandler> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<List<GameDto>> Handle(GetGamesDataQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting games data for month {Month}, tag: {TagId}, platform: {Platform}",
                request.Month, request.TagId, request.Platform);

            try
            {
                var games = await _gameRepository.GetGamesAsync(request.Month, request.TagId, request.Platform);

                _logger.LogDebug("Retrieved {GameCount} games from repository", games.Count);

                var gameDtos = games.Select(g => new GameDto()
                {
                    Id = g.Id,
                    SteamAppGameId = g.SteamAppGameId,
                    GameName = g.GameName,
                    DateOfRelease = g.DateOfRelease,
                    ImageRef = g.ImageRef,
                    ShopRef = g.ShopRef,
                    ShortDescription = g.ShortDescription
                });

                var result = gameDtos.ToList();

                _logger.LogInformation("Successfully mapped {GameDtoCount} game DTOs", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting games data");
                throw;
            }
        }
    }
}
