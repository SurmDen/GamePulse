using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Entites
{
    public class Game : EntityBase
    {
        public long SteamAppGameId { get; set; }

        public string GameName { get; set; } = string.Empty;

        public string DateOfRelease { get; set; } = string.Empty;

        public string ShopRef { get; set; } = string.Empty;

        public string ImageRef { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;

        public bool IsWindowsSupported { get; set; }

        public bool IsLinuxSupported { get; set; }

        public bool IsMacSupported { get; set; }

        public GameInfo GameInfo { get; set; }
    }
}
