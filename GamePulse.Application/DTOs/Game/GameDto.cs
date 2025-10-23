using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Application.DTOs.Game
{
    public class GameDto
    {
        public Guid Id { get; set; }

        public long SteamAppGameId { get; set; }

        public string GameName { get; set; } = string.Empty;

        public DateTime DateOfRelease { get; set; }

        public string ShopRef { get; set; } = string.Empty;

        public string ImageRef { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;
    }
}
