using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Entites
{
    public class GameInfo : EntityBase
    {
        public DateTime DateOfSearch { get; set; }

        public long FollowersCount { get; set; }

        public List<Genre> Genres { get; set; }

        public List<Tag> Tags { get; set; }

        public Game Game { get; set; }

        public Guid GameId { get; set; }
    }
}
