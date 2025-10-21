using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Entites
{
    public class GameInfo : EntityBase
    {
        public string GameName { get; set; } = string.Empty;

        public DateTime DateOfRelease { get; set; }

        public string ShopRef { get; set; } = string.Empty;

        public string ImageRef { get; set; } = string.Empty;
    }
}
