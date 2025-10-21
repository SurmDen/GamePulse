using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Entites
{
    public class Tag : EntityBase
    {
        public long SteamAppTagId { get; set; }

        public string TagName { get; set; } = string.Empty;

        public List<GameInfo> GameInfoModels { get; set; }
    }
}
