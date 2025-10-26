using GamePulse.Application.DTOs.Genre;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GamePulse.Application.Queries.Genre
{
    public class GetGenresDataByMonthQueryHandler : IRequestHandler<GetGenresDataByMonthQuery, List<MonthlyGenreDto>>
    {
        private readonly IGenreRepository _genreRepository;
        private readonly ILogger<GetGenresDataByMonthQueryHandler> _logger;

        public GetGenresDataByMonthQueryHandler(IGenreRepository genreRepository, ILogger<GetGenresDataByMonthQueryHandler> logger)
        {
            _genreRepository = genreRepository;
            _logger = logger;
        }

        public async Task<List<MonthlyGenreDto>> Handle(GetGenresDataByMonthQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to process GetGenresDataByMonthQuery for {MonthsCount} months, {GenresCount} genres per month", request.MonthsCount, request.GenresCount);

            if (request == null)
            {
                _logger.LogError("Request is null");
                throw new ArgumentNullException(nameof(request));
            }

            if (request.MonthsCount <= 0)
            {
                _logger.LogError("Invalid MonthsCount: {MonthsCount}", request.MonthsCount);

                throw new ArgumentException("MonthsCount must be greater than 0", nameof(request.MonthsCount));
            }

            if (request.GenresCount <= 0)
            {
                _logger.LogError("Invalid GenresCount: {GenresCount}", request.GenresCount);

                throw new ArgumentException("GenresCount must be greater than 0", nameof(request.GenresCount));
            }

            if (request.StartFromDate == DateTime.MinValue || request.StartFromDate == DateTime.MaxValue)
            {
                _logger.LogError("Invalid StartFromDate: {StartFromDate}", request.StartFromDate);

                throw new ArgumentException("StartFromDate must be a valid date", nameof(request.StartFromDate));
            }

            List<MonthlyGenreDto> monthlyGenreDtos = new List<MonthlyGenreDto>();
            var originalStartDate = request.StartFromDate;

            try
            {
                for (int i = 0; i < request.MonthsCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentDate = originalStartDate.AddMonths(-i);
                    _logger.LogDebug("Processing month {MonthNumber}: {Month}/{Year}",
                        i + 1, currentDate.Month, currentDate.Year);

                    try
                    {
                        var genres = await _genreRepository.GetTopGenresWithGamesAsync(
                            request.GenresCount, currentDate.Year, currentDate.Month);

                        _logger.LogDebug("Retrieved {GenreCount} genres for {Month}/{Year}",
                            genres.Count, currentDate.Month, currentDate.Year);

                        List<GenreDto> genresData = new List<GenreDto>();

                        foreach (var genre in genres)
                        {
                            try
                            {
                                var followersData = genre.Games.SelectMany(g => g.DatedGameInfo)
                                    .Where(info => info != null)
                                    .Select(info => info.FollowersCount)
                                    .ToList();

                                double avgFollowers = followersData.Any() ? followersData.Average() : 0;

                                genresData.Add(new GenreDto()
                                {
                                    GenreName = genre.GenreName,
                                    GenreId = genre.Id,
                                    GamesCount = genre.Games?.Count ?? 0,
                                    AvgFollowersCount = (int)Math.Round(avgFollowers)
                                });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error processing genre {GenreName} (ID: {GenreId})", genre.GenreName, genre.Id);

                                continue;
                            }
                        }

                        monthlyGenreDtos.Add(new MonthlyGenreDto()
                        {
                            Month = $"{currentDate.Month}/{currentDate.Year}",
                            GenresData = genresData
                        });

                        _logger.LogDebug("Successfully processed month {Month}/{Year} with {GenreDataCount} genre entries", currentDate.Month, currentDate.Year, genresData.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing month {Month}/{Year}",
                            currentDate.Month, currentDate.Year);

                        continue;
                    }
                }

                _logger.LogInformation("Successfully processed GetGenresDataByMonthQuery. Returned data for {ProcessedMonths} months", monthlyGenreDtos.Count);

                return monthlyGenreDtos;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetGenresDataByMonthQuery was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetGenresDataByMonthQueryHandler");
                throw;
            }
        }
    }
}
