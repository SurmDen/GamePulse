using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Core.Interfaces.Services
{
    public interface IDateParser
    {
        public DateTime ParseDateFromString(string date);
    }
}
