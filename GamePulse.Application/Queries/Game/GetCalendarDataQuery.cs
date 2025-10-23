using GamePulse.Application.DTOs.Game;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Queries.Game
{
    public class GetCalendarDataQuery : IRequest<CalendarDto>
    {
        public int Month { get; set; }

        public Guid? TagId { get; set; }

        public string? Platform { get; set; }
    }
}
