using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Entites
{
    public class Game : EntityBase
    {
        public long SteamAppGameId { get; set; }

        public string GameName { get; set; } = string.Empty;

        public DateTime DateOfRelease { get; set; }

        public string ShopRef { get; set; } = string.Empty;

        public string ImageRef { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;

        public bool IsWindowsSupported { get; set; }

        public bool IsLinuxSupported { get; set; }

        public bool IsMacSupported { get; set; }

        public List<Genre> Genres { get; set; }

        public List<Tag> Tags { get; set; }

        public List<GameInfo> DatedGameInfo { get; set; }

        [NotMapped]
        public long Followers { get; set; }
    }
}
