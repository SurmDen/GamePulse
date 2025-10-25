using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.Events
{
    public class GameSearchEvent : INotification
    {
        public Guid EventId { get; set; } = Guid.NewGuid();

        public DateTime OccuredAt { get; set; } = DateTime.UtcNow.Date;

        public int NeededMonth { get; set; }

        public int NeededYear { get; set; }
    }
}
