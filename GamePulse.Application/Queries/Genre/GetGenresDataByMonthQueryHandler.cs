using GamePulse.Application.DTOs.Genre;
using GamePulse.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Queries.Genre
{
    public class GetGenresDataByMonthQueryHandler : IRequestHandler<GetGenresDataByMonthQuery, List<MonthlyGenreDto>>
    {
        public GetGenresDataByMonthQueryHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        private readonly IGenreRepository _genreRepository;

        public async Task<List<MonthlyGenreDto>> Handle(GetGenresDataByMonthQuery request, CancellationToken cancellationToken)
        {
            List<MonthlyGenreDto> monthlyGenreDtos = new List<MonthlyGenreDto>();

            for (int i = 0; i < request.MonthsCount; i++)
            {
                request.StartFromDate = request.StartFromDate.AddMonths(- i);

                var genres = await _genreRepository.GetTopGenresWithGamesAsync(request.GenresCount, request.StartFromDate.Year, request.StartFromDate.Month);

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

                monthlyGenreDtos.Add(new MonthlyGenreDto()
                {
                    Month = $"{request.StartFromDate.Month}/{request.StartFromDate.Year}",
                    GenresData = genresData
                });
            }

            return monthlyGenreDtos;
        }
    }
}
