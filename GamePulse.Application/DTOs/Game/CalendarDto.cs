using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.DTOs.Game
{
    public class CalendarDto
    {
        public DateTime Month { get; set; }

        public List<DayDto>? Days { get; set; }
    }
}
