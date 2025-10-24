using GamePulse.Application.DTOs.Genre;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;

namespace GamePulse.Application.Queries.Genre
{
    public class GetTopGenresQueryHandler : IRequestHandler<GetTopGenresQuery, List<GenreDto>>
    {
        public GetTopGenresQueryHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        private readonly IGenreRepository _genreRepository;

        public async Task<List<GenreDto>> Handle(GetTopGenresQuery request, CancellationToken cancellationToken)
        {
            var genres = await _genreRepository.GetTopGenresWithGamesAsync(request.GenresCount);

            List<GenreDto> genresData = new List<GenreDto>();

            foreach (var genre in genres)
            {
                genresData.Add(new GenreDto()
                {
                    GenreName = genre.GenreName,
                    GenreId = genre.Id,
                    GamesCount = genre.Games.Count,
                    AvgFollowersCount = (int)genre.Games.SelectMany(g => g.DatedGameInfo).Average(i => i.FollowersCount)
                });
            }

            return genresData;
        }
    }
}
