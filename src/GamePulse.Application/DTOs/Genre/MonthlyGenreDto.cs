using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.DTOs.Genre
{
    public class MonthlyGenreDto
    {
        public string Month { get; set; } = string.Empty;

        public List<GenreDto> GenresData { get; set; }
    }
}
