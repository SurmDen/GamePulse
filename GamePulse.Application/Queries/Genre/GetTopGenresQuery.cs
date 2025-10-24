using GamePulse.Application.DTOs.Genre;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Queries.Genre
{
    public class GetTopGenresQuery : IRequest<List<GenreDto>>
    {
        public int GenresCount { get; set; }
    }
}
