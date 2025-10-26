using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Entites
{
    public class User : EntityBase
    {
        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
    }
}
