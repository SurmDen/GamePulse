using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Entites
{
    public class Platform : EntityBase
    {
        public string PlatformName { get; set; } = string.Empty;

        public List<Game> Games { get; set; }
    }
}
