using GamePulse.Application.DTOs.Game;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Queries.Game
{
    public class GetGamesDataQueryHandler : IRequestHandler<GetGamesDataQuery, List<GameDto>>
    {
        public GetGamesDataQueryHandler(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        private readonly IGameRepository _gameRepository;

        public async Task<List<GameDto>> Handle(GetGamesDataQuery request, CancellationToken cancellationToken)
        {
            var games = await _gameRepository.GetGamesAsync(request.Month, request.TagId, request.Platform);

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

            return gameDtos.ToList();
        }
    }
}
