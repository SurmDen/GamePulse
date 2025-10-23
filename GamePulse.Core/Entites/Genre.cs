using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Entites
{
    public class Genre : EntityBase
    {
        public string GenreName { get; set; } = string.Empty;

        public long SteamAppGenreId { get; set; }

        public List<Game> Games { get; set; }
    }
}
