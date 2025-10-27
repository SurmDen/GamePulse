using GamePulse.Application.Queries.Genre;
using GamePulse.Core.Interfaces.Repositories;
using GamePulse.Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamePulse.Web.Controllers
{
    [ApiController]
    [Route("api/v1/genres")]
    public class GenreController : ControllerBase
    {
        public GenreController(ILogger<GenreController> logger, IMediator mediator, IDateParser dateParser, IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
            _logger = logger;
            _mediator = mediator;
            _dateParser = dateParser;
        }

        private readonly ILogger<GenreController> _logger;
        private readonly IMediator _mediator;

        private readonly IGenreRepository _genreRepository;
        private readonly IDateParser _dateParser;

        [Authorize(Policy = "Bearer")]
        [HttpGet("{count:int}")]
        public async Task<IActionResult> GetTopGenresAsync(int count)
        {
            GetTopGenresQuery getTopGenresQuery = new GetTopGenresQuery()
            {
                GenresCount = count
            };

            try
            {
                var genres = await _mediator.Send(getTopGenresQuery);

                return Ok(genres);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying to get top genres");

                return Problem(statusCode: 400, title: "getting top genres data error", detail: "Error occured while trying to get top genres data");
            }
        }

        [Authorize(Policy = "Bearer")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetTopGenresStatisticsAsync(
            [FromQuery] int year,
            [FromQuery] int month,
            [FromQuery] int months_count,
            [FromQuery] int genres_count
            )
        {
            if (month < 1 || month > 12)
            {
                return BadRequest("Invalid month number");
            }

            GetGenresDataByMonthQuery query = new GetGenresDataByMonthQuery()
            {
                StartFromDate = new DateTime(year, month, 1),
                GenresCount = genres_count,
                MonthsCount = months_count
            };

            try
            {
                var statistics = await _mediator.Send(query);

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while trying to get top genres statistics");

                return Problem(statusCode: 400, title: "getting genres statistics data error", detail: "Error occured while trying to get genres statistics data");
            }
        }
    }
}
