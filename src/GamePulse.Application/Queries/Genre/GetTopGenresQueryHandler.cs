using GamePulse.Application.DTOs.Genre;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GamePulse.Application.Queries.Genre
{
    public class GetTopGenresQueryHandler : IRequestHandler<GetTopGenresQuery, List<GenreDto>>
    {
        private readonly IGenreRepository _genreRepository;
        private readonly ILogger<GetTopGenresQueryHandler> _logger;

        public GetTopGenresQueryHandler(IGenreRepository genreRepository, ILogger<GetTopGenresQueryHandler> logger)
        {
            _genreRepository = genreRepository;
            _logger = logger;
        }

        public async Task<List<GenreDto>> Handle(GetTopGenresQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting top {GenresCount} genres", request.GenresCount);

            try
            {
                var genres = await _genreRepository.GetTopGenresWithGamesAsync(request.GenresCount);

                _logger.LogDebug("Retrieved {GenreCount} genres from repository", genres.Count);

                List<GenreDto> genresData = new List<GenreDto>();

                foreach (var genre in genres)
                {
                    try
                    {
                        var avgFollowers = (int)genre.Games.SelectMany(g => g.DatedGameInfo).Average(i => i.FollowersCount);

                        genresData.Add(new GenreDto()
                        {
                            GenreName = genre.GenreName,
                            GenreId = genre.Id,
                            GamesCount = genre.Games.Count,
                            AvgFollowersCount = avgFollowers
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error processing genre {GenreName}, skipping", genre.GenreName);
                        continue;
                    }
                }

                _logger.LogInformation("Successfully processed {ProcessedGenres} genres", genresData.Count);

                return genresData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top genres");
                throw;
            }
        }
    }
}
