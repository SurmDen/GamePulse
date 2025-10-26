using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.DTOs.Genre
{
    public class GenreDto
    {
        public string GenreName { get; set; } = string.Empty;

        public Guid GenreId { get; set; }

        public int GamesCount { get; set; }

        public int AvgFollowersCount { get; set; }
    }
}
